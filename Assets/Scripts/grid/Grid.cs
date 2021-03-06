﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

/*
 * This singleton represents a dynamic 3D Grid. It's instantiated as a flat 2D grid, which then expands upward as buildings are added.
 * The Grid is implemented using a dictionary, as a compromise between memory consumption and speed.
 * (Node[ , , ] would be faster, but it would contain loads of empty objects)
 * */
public class Grid : MonoBehaviour {
    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private int nodeInterval;
    [SerializeField]
    private int nodeHeight;

    private Dictionary<Vector3, Node> nodes;

    [SerializeField]
    private int maxHeight;
    [SerializeField]
    private int width;
    [SerializeField]
    private int length;

    private int borderXLower;
    private int borderXUpper;
    private int borderZLower;
    private int borderZUpper;

    private Dictionary<StructureType, List<StructureType>> stackOrder = new Dictionary<StructureType, List<StructureType>>();

    public void Awake() {
        SaveLoad.SaveState += SaveState;
        SaveLoad.LoadStateEarly += LoadState;
        SaveLoad.InitGameEarly += FirstLaunch;

        nodes = new Dictionary<Vector3, Node>();

        //define what can be built on what
        // key/value => structure/structures that can be built on it
        stackOrder.Add(StructureType.Hut, new List<StructureType> { StructureType.Hut, StructureType.Field });
        stackOrder.Add(StructureType.Storage, new List<StructureType> { StructureType.Hut, StructureType.Field });
        stackOrder.Add(StructureType.Sawmill, new List<StructureType>());
        stackOrder.Add(StructureType.Field, new List<StructureType>());
        stackOrder.Add(StructureType.Statue, new List<StructureType>());
        stackOrder.Add(StructureType.Outpost, new List<StructureType>());
    }

    private Node CreateNode(Vector3 pos) {
        GameObject obj = (GameObject)Instantiate(nodePrefab, pos, Quaternion.identity);
        obj.transform.parent = transform;
        Node node = obj.GetComponent<Node>();
        return node;
    }

    public void LevelUp(int level) {
        maxHeight = level;
        int w = width + level - 1;
        int l = length + level - 1;

        if (level % 2 == 0) {  //expand south & east every other time
            borderXUpper += nodeInterval;
            borderZLower -= nodeInterval;

            for (int i = 0; i < w; i++) {
                Vector3 posZ = new Vector3(borderXLower + (i * nodeInterval), 0, borderZLower);
                Node node = CreateNode(posZ);
                nodes.Add(posZ, node);
            }
            for (int i = 1; i < l; i++) { // avoid creating corner twice by starting at l=1
                Vector3 posX = new Vector3(borderXUpper, 0, borderZLower + (i * nodeInterval));
                Node node = CreateNode(posX);
                nodes.Add(posX, node);
            }
        }
        else {  //expand north & west every other time
            borderXLower -= nodeInterval;
            borderZUpper += nodeInterval;

            for (int i = 0; i < w; i++) {
                Vector3 posZ = new Vector3(borderXLower + (i * nodeInterval), 0, borderZUpper);
                Node node = CreateNode(posZ);
                nodes.Add(posZ, node);
            }
            for (int i = 1; i < l; i++) { // avoid creating corner twice by starting at l=1
                Vector3 posX = new Vector3(borderXLower, 0, borderZUpper - (i * nodeInterval));
                Node node = CreateNode(posX);
                nodes.Add(posX, node);
            }
        }
    }

    //draws a highlight-square around nodes that are valid build positions for the specified structure type
    public void HighLightValidNodes(Node node) {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            if (n.hasComponent) { continue; }
            if (n.lowerNode != null && !IsNodeCompatible(n.lowerNode, node.component)) { continue; }
            if (n.IsInSameStack(node)) { continue; } //don't highlight in same stack
            if (n.transform.position.y > (maxHeight - 1) * nodeHeight) { continue; } //height restriction
            n.HighLight();
        }
    }
    public void UnHighlightNodes() {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            n.UnHighLight();
        }
    }

    public void HighlightStack(Node node) {
        while (node.hasComponent) {
            node.component.HighLight();
            node = node.upperNode;
        }
    }
    public void UnHighlightStack(Node node) {
        while (node.hasComponent) {
            node.component.UnHighLight();
            node = node.upperNode;
        }
    }

    public bool HasRoomForComponent(GridComponent comp) {
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
        int y = 0; //only consider ground level positions here

        //cehck if the position is outside grid boundaries and apply corrections
        if (nearestX < borderXLower) { nearestX = borderXLower; }
        if (nearestX > borderXUpper) { nearestX = borderXUpper; }
        if (nearestZ < borderZLower) { nearestZ = borderZLower; }
        if (nearestZ > borderZUpper) { nearestZ = borderZUpper; }

        Vector3 nearestPosition = new Vector3(nearestX, y, nearestZ);
        Node nearestNode = getNodeByPosition(nearestPosition);
        if (nearestNode == null) { return curNode; }
        if (nearestNode.IsInSameStack(curNode)) { return curNode; }
        if (!nearestNode.hasComponent) { return nearestNode; }

        Node highestOccupiedNode = nearestNode.GetTopOfStack();
        if (highestOccupiedNode.transform.position.y >= (maxHeight - 1) * nodeHeight) {//maxheight-1 because positions start at 0
            return curNode;
        }
        if (IsNodeCompatible(highestOccupiedNode, curNode.component)) {
            return highestOccupiedNode.upperNode;
        }
        return curNode;
    }

    //finds a random free node in the grid
    private Node FindFreeNode(GridComponent comp) {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;

        //at first, search bottom layer only
        foreach (Node n in valueColl) {
            if (n.transform.position.y == 0 && !n.hasComponent) { return n; }
        }
        //then search any layer
        foreach (Node n in valueColl) {
            if (n.lowerNode != null && !n.hasComponent && IsNodeCompatible(n.lowerNode, comp)) {
                if (n.transform.position.y < (maxHeight * nodeHeight)) { return n; }
            }
        }
        return null;
    }

    ////re-adds a component to the grid when a savegame has been loaded
    public void ReAttachComponent(GridComponent component) {
        Node node = getNodeByPosition(component.transform.position);
        node.AttachComponent(component);
    }

    //adds a component to the grid, at a free node
    public void AttachComponent(GridComponent component) {
        Node node = FindFreeNode(component);
        node.AttachComponent(component);
        component.transform.position = node.transform.position;
        if (node.lowerNode != null) { node.lowerNode.setUpperNode(node); }

        //add a free node on top of the building
        Node newNode = CreateNode(node.transform.position);
        newNode.MoveUp(nodeHeight);
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
        if (rootNode != null && rootNode.hasComponent) {
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
            while (nextUp.hasComponent) {
                nextUp = nextUp.upperNode;
                nodes.Remove(nextUp.transform.position);
                nextUp.MoveToNewPos(rootNode.transform.position);
                nextUp.MoveUp(nodeHeight * stackHeight);
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

    //calculate borders so that the center node is at the center of the grid
    private void CalculateBorders() {
        int gridCenterX = (int)transform.position.x;
        int gridCenterZ = (int)transform.position.z;
        borderXLower = gridCenterX - ((width / 2) * nodeInterval);
        borderXUpper = gridCenterX + ((width / 2) * nodeInterval);
        borderZLower = gridCenterZ - ((length / 2) * nodeInterval);
        borderZUpper = gridCenterZ + ((length / 2) * nodeInterval);

        //offset center node by 1 on both axes if node count is even
        if (width % 2 == 0) { borderXUpper -= 1 * nodeInterval; }
        if (length % 2 == 0) { borderZUpper -= 1 * nodeInterval; }
    }

    //initialize the grid by creating 1 layer of nodes
    private void FirstLaunch() {
        CalculateBorders();
        for (int i = borderXLower; i <= borderXUpper; i += nodeInterval) {
            for (int j = borderZLower; j <= borderZUpper; j += nodeInterval) {
                Vector3 pos = new Vector3(i, 0, j);
                Node node = CreateNode(pos);
                nodes.Add(pos, node);
            }
        }
    }

    //brute force. could be improved.
    private void RebuildNodeReferences() {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node node in valueColl) {
            if (node.upperNode == null) {
                Vector3 upperPos = new Vector3(node.transform.position.x, node.transform.position.y + nodeHeight, node.transform.position.z);
                node.setUpperNode(getNodeByPosition(upperPos));
            }
            if (node.lowerNode == null) {
                Vector3 lowerPos = new Vector3(node.transform.position.x, node.transform.position.y - nodeHeight, node.transform.position.z);
                node.setLowerNode(getNodeByPosition(lowerPos));
            }
        }
    }




    private void SaveState(SaveLoad.GameState gamestate) {
        GridState myState = new GridState();
        myState.maxHeight = maxHeight;
        myState.borderXLower = borderXLower;
        myState.borderXUpper = borderXUpper;
        myState.borderZLower = borderZLower;
        myState.borderZUpper = borderZUpper;

        Dictionary<Vector3, Node>.KeyCollection keyColl = nodes.Keys;
        foreach (Vector3 pos in keyColl) {
            myState.nodes.Add(new GridState.NodeRepresentation(pos));
        }
        gamestate.gridState = myState;
    }

    private void LoadState(SaveLoad.GameState gamestate) {
        maxHeight = gamestate.gridState.maxHeight;
        borderXLower = gamestate.gridState.borderXLower;
        borderXUpper = gamestate.gridState.borderXUpper;
        borderZLower = gamestate.gridState.borderZLower;
        borderZUpper = gamestate.gridState.borderZUpper;

        foreach (GridState.NodeRepresentation n in gamestate.gridState.nodes) {
            Node node = CreateNode(n.GetPos());
            nodes.Add(n.GetPos(), node);
        }
        RebuildNodeReferences();
    }

    [System.Serializable]
    public class GridState {
        public int maxHeight;
        public int borderXLower;
        public int borderXUpper;
        public int borderZLower;
        public int borderZUpper;
        public List<NodeRepresentation> nodes;
        public GridState() {
            nodes = new List<NodeRepresentation>();
        }

        [System.Serializable]
        public class NodeRepresentation {
            public float[] pos;

            public NodeRepresentation(Vector3 pos) {
                this.pos = new float[] { pos.x, pos.y, pos.z };
            }
            public Vector3 GetPos() {
                return new Vector3(pos[0], pos[1], pos[2]);
            }
        }
    }
}