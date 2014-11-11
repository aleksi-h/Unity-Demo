using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This singleton represents a dynamic 3D Grid. It's instantiated as a flat 2D grid, which then expands upward as buildings are added.
 * The Grid is implemented using a dictionary, as a compromise between memory consumption and speed.
 * (Node[ , , ] would be faster, but it would contain loads of empty objects)
 * */
public class Grid : Singleton<Grid> {
    public GameObject nodePrefab;

    //width & depth in nodes
    public int gridLengthX;
    public int gridLengthZ;
    public int nodeInterval;

    private Dictionary<Vector3, Node> nodes;

    private int borderXLower;
    private int borderXUpper;
    private int borderZLower;
    private int borderZUpper;

    private Dictionary<StructureType, List<StructureType>> stackOrder = new Dictionary<StructureType, List<StructureType>>();

    public override void Awake() {
        base.Awake();

        //calculate borders so that the center node is at the center of the grid
        int gridCenterX = (int)transform.position.x;
        int gridCenterZ = (int)transform.position.z;
        borderXLower = gridCenterX - ((gridLengthX / 2) * nodeInterval);
        borderXUpper = gridCenterX + ((gridLengthX / 2) * nodeInterval);
        borderZLower = gridCenterZ - ((gridLengthZ / 2) * nodeInterval);
        borderZUpper = gridCenterZ + ((gridLengthZ / 2) * nodeInterval);

        //offset center node by 1 on both axes if node count is even
        if (gridLengthX % 2 == 0) { borderXUpper -= 1 * nodeInterval; }
        if (gridLengthZ % 2 == 0) { borderZUpper -= 1 * nodeInterval; }

        //initialize the grid by creating 1 layer of nodes
        nodes = new Dictionary<Vector3, Node>();
        for (int i = borderXLower; i <= borderXUpper; i += nodeInterval) {
            for (int j = borderZLower; j <= borderZUpper; j += nodeInterval) {
                Vector3 pos = new Vector3(i, 0, j);
                Node node = CreateNode(pos);
                node.HideHighLight();
                nodes.Add(pos, node);
            }
        }

        //define what can be built on what
        // key/value => structure/structures that can be built on it
        stackOrder.Add(StructureType.Hut, new List<StructureType> { StructureType.Hut, StructureType.Field });
        stackOrder.Add(StructureType.Storage, new List<StructureType> { StructureType.Hut, StructureType.Field });
        stackOrder.Add(StructureType.Sawmill, new List<StructureType> { StructureType.Field });
        stackOrder.Add(StructureType.Field, new List<StructureType>());
        stackOrder.Add(StructureType.Special, new List<StructureType>());
    }

    private Node CreateNode(Vector3 pos) {
        GameObject obj = (GameObject)Instantiate(nodePrefab, pos, Quaternion.identity);
        obj.transform.parent = transform;
        Node node = obj.GetComponent<Node>();
        node.HideHighLight();
        return node;
    }

    //draws a highlight-square around nodes that are valid build positions for the specified structure type
    public void HighLightValidNodes(Node node) {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            if (n.isOccupied) { continue; }
            if (n.lowerNode != null && !IsNodeCompatible(n.lowerNode, node.component)) { continue; }
            if (n.lowerNode != null && n.lowerNode == node) { continue; } //don't highlight self
            n.HighLight();
        }
    }
    public void UnHighlightNodes() {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            n.HideHighLight();
        }
    }

    public void HighlightStack(Node node) {
        while (node.isOccupied) {
            node.component.HighLight();
            node = node.upperNode;
        }
    }
    public void UnHighlightStack(Node node) {
        while (node.isOccupied) {
            node.component.HideHighLight();
            node = node.upperNode;
        }
    }

    public bool HasRoom(GridComponent comp) {
        if (FindFreeNode(comp) != null) { return true; }
        return false;
    }

    public void RequestNewPosition(Node node, Vector3 newPos) {
        Node nearestNode = GetNearestValidNode(node, newPos);
        if (!node.Equals(nearestNode)) {
            MoveStack(node, nearestNode.transform.position);
        }
    }

    //returns the newPos rounded to nearest valid node location. 
    //if the nearest location is invalid for this structure type, returns currentPos
    private Node GetNearestValidNode(Node curNode, Vector3 targetPos) {
        //round the position to nearest node position
        int nearestX = (int)Mathf.Round(targetPos.x / nodeInterval) * nodeInterval;
        int nearestZ = (int)Mathf.Round(targetPos.z / nodeInterval) * nodeInterval;
        int y = 0; //only considering ground level positions here

        //cehck if the position is outside grid boundaries and apply corrections
        if (nearestX < borderXLower) { nearestX = borderXLower; }
        if (nearestX > borderXUpper) { nearestX = borderXUpper; }
        if (nearestZ < borderZLower) { nearestZ = borderZLower; }
        if (nearestZ > borderZUpper) { nearestZ = borderZUpper; }

        Vector3 nearestPosition = new Vector3(nearestX, y, nearestZ);
        Node nearestNode = getNodeByPosition(nearestPosition);
        if (nearestNode == null) { return curNode; }
        if (!nearestNode.isOccupied) { return nearestNode; }

        Node highestOccupiedNode = nearestNode.GetTopOfStack();
        if (IsNodeCompatible(highestOccupiedNode, curNode.component)) {
            return highestOccupiedNode.upperNode;
        }
        else { return curNode; }
    }

    //finds a random free node in the grid
    private Node FindFreeNode(GridComponent comp) {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;

        //at first, search bottom layer only
        foreach (Node n in valueColl) {
            if (n.transform.position.y == 0 && !n.isOccupied) {
                return n;
            }
        }
        //then search any layer
        foreach (Node n in valueColl) {
            if (n.lowerNode != null && !n.isOccupied && IsNodeCompatible(n.lowerNode, comp)) {
                return n;
            }
        }
        return null;
    }

    //adds a component to the grid, at a free node
    public void AttachComponent(GridComponent component) {
        Node node = FindFreeNode(component);
        node.AttachComponent(component);
        component.transform.position = node.transform.position;
        if (node.lowerNode != null) { node.lowerNode.setUpperNode(node); }

        //add a free node on top of the building
        Node newNode = CreateNode(node.transform.position);
        newNode.MoveUp(nodeInterval);
        newNode.lowerNode = node;
        node.upperNode = newNode;
        nodes.Add(newNode.transform.position, newNode);
    }

    //removes a component from the grid
    public void DetachComponent(Node node) {
        node.DetachComponent();
        RemoveNode(node.upperNode);
    }

    public void MoveStack(Node rootNode, Vector3 targetPos) {
        if (rootNode != null && rootNode.isOccupied) {
            Node targetNode = getNodeByPosition(targetPos);
            Vector3 rootOriginalPos = rootNode.transform.position;

            //switch rootnode positions
            targetNode.MoveToNewPos(rootOriginalPos);
            rootNode.MoveToNewPos(targetPos);

            //attach nodes to their new stacks
            if (rootNode.lowerNode != null) { rootNode.lowerNode.setUpperNode(targetNode); }
            if (targetNode.lowerNode != null) { targetNode.lowerNode.setUpperNode(rootNode); }
            Node aux = targetNode.lowerNode;
            targetNode.setLowerNode(rootNode.lowerNode);
            rootNode.setLowerNode(aux);

            //update nodes
            bool rm1 = nodes.Remove(rootOriginalPos);
            bool rm2 = nodes.Remove(targetPos);
            nodes.Add(targetNode.transform.position, targetNode);
            nodes.Add(rootNode.transform.position, rootNode);

            //move the rest of the stack
            int stackHeight = 1;
            Node nextUp = rootNode;
            while (nextUp.isOccupied) {
                nextUp = nextUp.upperNode;
                nodes.Remove(nextUp.transform.position);
                nextUp.MoveToNewPos(rootNode.transform.position);
                nextUp.MoveUp(nodeInterval * stackHeight);
                nodes.Add(nextUp.transform.position, nextUp);
                stackHeight++;
            }
        }
    }

    //decides whether 2 nodes can be stacked, based on the rules in "stackOrder" dictionary
    private bool IsNodeCompatible(Node node, GridComponent comp) {
        List<StructureType> compatibleStructures;
        stackOrder.TryGetValue(node.component.type, out compatibleStructures);

        if (compatibleStructures.Contains(comp.type)) { return true; }
        else { return false; }
    }

    private Node getNodeByPosition(Vector3 pos) {
        Node node = null;
        nodes.TryGetValue(pos, out node);
        return node;
    }

    private void RemoveNode(Node node) {
        if (node != null) {
            node.Destroy();
            nodes.Remove(node.transform.position);
        }
    }
}