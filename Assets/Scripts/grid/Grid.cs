using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This singleton represents a dynamic 3D Grid. It's instantiated as a flat 2D grid, which then expands upward as buildings are added.
 * 
 * NOTE: If the List<Node> implementation turns out to be too slow because of all the iteration, 
 * it can be replaced by a simple Node[ , , ] array, 
 * where indices are accessed by [posX/interval, posY/interval, posZ/interval]
 * */
public class Grid : Singleton<Grid>
{
    public Material material;

    //width & depth in nodes
    public int gridLengthX;
    public int gridLengthZ;

    public int nodeInterval;

    private List<Node> nodes;
    private List<GameObject> highlightLines;

    private int borderXLower;
    private int borderXUpper;
    private int borderZLower;
    private int borderZUpper;

    private Dictionary<StructureType, List<StructureType>> stackOrder = new Dictionary<StructureType, List<StructureType>>();

    public override void Awake()
    {
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

        //initialize the grid by creating the first layer of nodes
        highlightLines = new List<GameObject>();
        nodes = new List<Node>();
        for (int i = borderXLower; i <= borderXUpper; i+=nodeInterval)
        {
            for (int j = borderZLower; j <= borderZUpper; j+=nodeInterval)
            {
                Node node = new Node(new Vector3(i, 0, j), false);
                nodes.Add(node);
            }
        }

        //define what can be built on what
        // key/value => structure/structures that can be built on it
        stackOrder.Add(StructureType.hut, new List<StructureType> { StructureType.hut, StructureType.field });
        stackOrder.Add(StructureType.storage, new List<StructureType> { StructureType.hut, StructureType.field });
        stackOrder.Add(StructureType.sawmill, new List<StructureType> { StructureType.field });
        stackOrder.Add(StructureType.field, new List<StructureType>());
    }

    //draws a highlight-square around nodes that are valid build positions for the specified structure type
    public void HighLightValidNodes(StructureType type)
    {
        float radius = nodeInterval / 2.5f;
        Vector3 offset1 = new Vector3(radius, 0, radius);
        Vector3 offset2 = new Vector3(-radius, 0, radius);
        //Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));

        //create a shitload of objects to draw a square around each free node (might need a better solution than LineRenderer)
        foreach (Node node in nodes)
        {
            if (node.occupied) { continue; }
            if (node.nextNodeDown != null && !IsNodeCompatible(node.nextNodeDown, type)) { continue; }

            Vector3 elevatedNodePosition = new Vector3(node.position.x, node.position.y + 0.1f, node.position.z);
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            LineRenderer line = obj.AddComponent<LineRenderer>();
            line.material = material;
            line.SetWidth(0.2F, 0.2F);
            line.SetVertexCount(5);

            Vector3 corner1 = elevatedNodePosition - offset1;
            Vector3 corner2 = elevatedNodePosition - offset2;
            Vector3 corner3 = elevatedNodePosition + offset1;
            Vector3 corner4 = elevatedNodePosition + offset2;
            line.SetPosition(0, corner1);
            line.SetPosition(1, corner2);
            line.SetPosition(2, corner3);
            line.SetPosition(3, corner4);
            line.SetPosition(4, corner1);
            highlightLines.Add(obj);
        }
    }

    public void HideHighlight()
    {
        if (highlightLines!=null)
        {
            while (highlightLines.Count > 0)
            {
                Destroy(highlightLines[0]);
                highlightLines.RemoveAt(0);
            }
        }
    }

    //returns the newPos rounded to nearest valid node location. 
    //if the nearest location is invalid for this structure type, returns currentPos
    public Vector3 GetNearestValidNode(Vector3 currentPos, Vector3 newPos, StructureType type)
    {
        //round the position to nearest node position
        int nearestX = (int)Mathf.Round(newPos.x / nodeInterval) * nodeInterval;
        int nearestZ = (int)Mathf.Round(newPos.z / nodeInterval) * nodeInterval;
        int y = 0; //only considering ground level positions here

        //cehck if the position is outside grid boundaries
        if (nearestX < borderXLower) { nearestX = borderXLower; }
        if (nearestX > borderXUpper) { nearestX = borderXUpper; }
        if (nearestZ < borderZLower) { nearestZ = borderZLower; }
        if (nearestZ > borderZUpper) { nearestZ = borderZUpper; }

        Vector3 newPosition = new Vector3(nearestX, y, nearestZ);

        if (!IsNodeOccupied(newPosition)) { return newPosition; }
        else 
        {
            Node topNode = GetTopmostStructure(newPosition);
            if (IsNodeCompatible(topNode, type))
            {
                return GetTopmostStructure(newPosition).nextNodeUp.position;
            }
            else { return currentPos; }
        }
    }

    //adds a building to the node
    public void BuildToNode(Vector3 pos, StructureType type)
    {
        Node node = getNodeByPosition(pos);
        if (node != null)
        {
            node.occupied = true;
            node.structureType = type;
        }
        if (node.nextNodeDown != null)
        {
            node.nextNodeDown.nextNodeUp = node;
        }

        //add a free node on top of the building
        Node newNode = new Node(new Vector3(pos.x, pos.y + nodeInterval, pos.z), false);
        newNode.nextNodeDown = node;
        node.nextNodeUp = newNode;
        AddNode(newNode);
    }

    //removes any building from the node
    public void RemoveFromNode(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        if (node != null)
        {
            node.occupied = false;
            RemoveNode(node.nextNodeUp);
        }
    }

    //only the topmost building is movable/removable
    public bool IsBuildingMoveable(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        if (node.nextNodeUp == null) { return false; }
        if (node.nextNodeUp.occupied) { return false; }
        return true;
    }

    private bool IsNodeOccupied(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        if (node != null)
        {
            return node.occupied;
        }
        Debug.LogError("Node not found");
        return false;
    }

    //decides if "newStructure" can be built in "node", based on the rules written in "stackOrder" dictionary
    private bool IsNodeCompatible(Node node, StructureType newStructure)
    {
        StructureType lowerStructure = node.structureType;
        List<StructureType> compatibleStructures;
        stackOrder.TryGetValue(lowerStructure, out compatibleStructures);
        if (compatibleStructures.Contains(newStructure))
        {
            return true;
        }
        else { return false; }
    }

    //returns the highest occupied node
    private Node GetTopmostStructure(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        while (node.nextNodeUp.occupied)
        {
            node = node.nextNodeUp;
        }
        return node;
    }
    
    private Node getNodeByPosition(Vector3 pos)
    {
        foreach (Node node in nodes)
        {
            if (pos.Equals(node.position))
            {
                return node;
            }
        }
        return null;
    }

    private void AddNode(Node node)
    {
        nodes.Add(node);
    }

    private void RemoveNode(Node node)
    {
        if (node != null)
        {
            nodes.Remove(node);
        }
    }
}