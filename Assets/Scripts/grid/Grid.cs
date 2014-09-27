using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 
 * */
public class Grid : Singleton<Grid>
{
    public Material material;

    private List<Node> nodes;
    private List<GameObject> highlightLines;

    //width & depth in nodes
    public int gridLengthX;
    public int gridLengthZ;

    private int borderXLower;
    private int borderXUpper;
    private int borderZLower;
    private int borderZUpper;
    
    private int nodeInterval = 5;

    void Start()
    {
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
    }

    public void HighLightFreeNodes()
    {
        Vector3 offset1 = new Vector3(2, 0, 2);
        Vector3 offset2 = new Vector3(-2, 0, 2);
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));

        //create a shitload of objects to draw a square around each free node
        foreach (Node node in nodes)
        {
            Vector3 elevatedNodePosition = new Vector3(node.position.x, node.position.y+0.1f, node.position.z);
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

    public Vector3 GetNearestFreeNode(float x, float y, float z)
    {
        //round the position to match node positions
        int nearestX = (int)Mathf.Round(x / 5) * nodeInterval;
        int nearestZ = (int)Mathf.Round(z / 5) * nodeInterval;

        //restrict buildings movement inside the grid
        if (nearestX < borderXLower) { nearestX = borderXLower; }
        if (nearestX > borderXUpper) { nearestX = borderXUpper; }
        if (nearestZ < borderZLower) { nearestZ = borderZLower; }
        if (nearestZ > borderZUpper) { nearestZ = borderZUpper; }

        Vector3 pos = new Vector3(nearestX, y, nearestZ);
        if (IsNodeOccupied(pos))
        {
            return GetTopOfStack(pos);
        }
        else
        {
            return pos;
        }
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

    private Vector3 GetTopOfStack(Vector3 pos)
    {
        foreach (Node node in nodes)
        {
            if (node.position.x == pos.x && node.position.z == pos.z && !node.occupied)
            {
                return node.position;
            }
        }
        Debug.LogError("No free node on top of the building");
        return new Vector3(0, 0, 0);
    }

    public void BuildToNode(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        Node nextNodeDown = getNodeByPosition(new Vector3(pos.x, pos.y - nodeInterval, pos.z));

        if (node != null)
        {
            node.occupied = true;
        }
        if (nextNodeDown != null)
        {
            nextNodeDown.nextNodeUp = node;
        }

        //add a free node on top of the building
        Node newNode = new Node(new Vector3(pos.x, pos.y + nodeInterval, pos.z), false);
        node.nextNodeUp = newNode;
        AddNode(newNode);
    }

    public void RemoveFromNode(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        if (node != null)
        {
            node.occupied = false;
            RemoveNode(node.nextNodeUp);
        }
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

    public bool IsBuildingTopMost(Vector3 pos)
    {
        Node node = getNodeByPosition(pos);
        if (node.nextNodeUp == null) { return false; }
        if (node.nextNodeUp.occupied) { return false; }
        return true;
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