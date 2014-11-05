using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This singleton represents a dynamic 3D Grid. It's instantiated as a flat 2D grid, which then expands upward as buildings are added.
 * The Grid is implemented using a dictionary, as a compromise between memory consumption and speed.
 * (Node[ , , ] would be faster, but it would contain loads of empty objects)
 * */
public class Grid : Singleton<Grid> {
    public Material material;
    public GameObject nodePrefab;

    //width & depth in nodes
    public int gridLengthX;
    public int gridLengthZ;

    public int nodeInterval;

    //private List<Node> nodes;
    private Dictionary<Vector3, Node> nodes;
    private LayerMask structureLayerMask = 1 << 10;
    private LayerMask groundLayerMask = 1 << 11;

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

    public void Start() {
        InputManager.OnLongTap += OnLongTap;
    }

    private Node curStackRoot;
    private bool registeredToTapEvent;
    private Vector3 rootPosBeforeMove;

    private void OnLongTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
            Vector3 hitPos = hit.collider.gameObject.transform.position;
            curStackRoot = getNodeByPosition(hitPos);
            BaseStructure structureBase = curStackRoot.structure.GetComponent<BaseStructure>();
            if (!registeredToTapEvent) {
                InputManager.OnTap += OnTap;
                registeredToTapEvent = true;
                rootPosBeforeMove = curStackRoot.transform.position;
                GUIManager.Instance.ShowPlacementGUI(curStackRoot.structure, structureBase.Type);
            }
        }
    }
    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
            Vector3 newPos = GetNearestValidNode(curStackRoot, hit.point);//take node as arg
            if (!curStackRoot.transform.position.Equals(newPos)) {
                MoveStack(curStackRoot.transform.position, newPos);
            }
        }
    }
    public void startMove(GameObject obj) {
        curStackRoot = getNodeByPosition(obj.transform.position);
        if (!registeredToTapEvent) {
            InputManager.OnTap += OnTap;
            registeredToTapEvent = true;
        }
    }
    public void FinishMove() {
        if (registeredToTapEvent) { InputManager.OnTap -= OnTap; }
        registeredToTapEvent = false;
    }
    public void CancelMove() {
        if (registeredToTapEvent) { InputManager.OnTap -= OnTap; }
        registeredToTapEvent = false;
        if (!curStackRoot.transform.position.Equals(rootPosBeforeMove)) {
            MoveStack(curStackRoot.transform.position, rootPosBeforeMove);
        }
    }

    private Node CreateNode(Vector3 pos) {
        GameObject obj = (GameObject)Instantiate(nodePrefab, pos, Quaternion.identity);
        return obj.GetComponent<Node>();
    }

    //draws a highlight-square around nodes that are valid build positions for the specified structure type
    public void HighLightValidNodes(StructureType type) {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            if (n.isOccupied) { continue; }
            if (n.nextNodeDown != null && !IsNodeCompatible(n.nextNodeDown, type)) { continue; }
            n.HighLight();
        }
    }

    public void HideHighlight() {
        Dictionary<Vector3, Node>.ValueCollection valueColl = nodes.Values;
        foreach (Node n in valueColl) {
            n.HideHighLight();
        }
    }

    //returns the newPos rounded to nearest valid node location. 
    //if the nearest location is invalid for this structure type, returns currentPos
    public Vector3 GetNearestValidNode(Node curNode, Vector3 newPos) {
        Vector3 currentPos = curNode.transform.position;
        //round the position to nearest node position
        int nearestX = (int)Mathf.Round(newPos.x / nodeInterval) * nodeInterval;
        int nearestZ = (int)Mathf.Round(newPos.z / nodeInterval) * nodeInterval;
        int y = 0; //only considering ground level positions here

        //cehck if the position is outside grid boundaries and apply corrections
        if (nearestX < borderXLower) { nearestX = borderXLower; }
        if (nearestX > borderXUpper) { nearestX = borderXUpper; }
        if (nearestZ < borderZLower) { nearestZ = borderZLower; }
        if (nearestZ > borderZUpper) { nearestZ = borderZUpper; }

        Vector3 newPosition = new Vector3(nearestX, y, nearestZ);
        if (!IsNodeOccupied(newPosition)) { return newPosition; }

        Node highestBuiltNode = GetTopmostStructure(newPosition);
        Debug.Log("stack " + newPosition + " top structure " + highestBuiltNode.transform.position);
        if (IsNodeCompatible(highestBuiltNode, curNode.structure.GetComponent<BaseStructure>().Type)) {
            return highestBuiltNode.nextNodeUp.transform.position;
        }
        else { return currentPos; }
    }

    //adds a building to the node
    public void BuildToNode(Vector3 pos, GameObject structure) {
        Node node = getNodeByPosition(pos);
        if (node == null) { return; }

        node.isOccupied = true;
        //node.structure = structure;
        node.AttachStructure(structure);
        node.structureType = structure.GetComponent<BaseStructure>().Type;
        if (node.nextNodeDown != null) { node.nextNodeDown.nextNodeUp = node; }

        //add a free node on top of the building
        Vector3 newPos = new Vector3(pos.x, pos.y + nodeInterval, pos.z);
        Node newNode = CreateNode(newPos);
        newNode.nextNodeDown = node;
        node.nextNodeUp = newNode;
        nodes.Add(newPos, newNode);
    }

    //removes building from the node at "pos"
    public void RemoveFromNode(Vector3 pos) {
        Node node = getNodeByPosition(pos);
        if (node != null) {
            node.isOccupied = false;
            node.DetachStructure();
            RemoveNode(node.nextNodeUp);
        }
    }

    public void MoveStack(Vector3 oldRootPos, Vector3 newRootPos) {//pysyykö node tallessa muualla kuin dictionaryssä?
        Node oldPosRootNode = getNodeByPosition(oldRootPos);
        if (oldPosRootNode != null && oldPosRootNode.isOccupied) {

            //vaihda nodejen paikkaa
            Node newPosRootNode = getNodeByPosition(newRootPos);
            newPosRootNode.MoveToNewPos(oldRootPos);//yhdistä node alempaan
            oldPosRootNode.MoveToNewPos(newRootPos);//yhdistä alempaan

            //yhdistä nodet uusiin stäckeihin
            if (oldPosRootNode.nextNodeDown != null) { oldPosRootNode.nextNodeDown.nextNodeUp = newPosRootNode; }
            if (newPosRootNode.nextNodeDown != null) { newPosRootNode.nextNodeDown.nextNodeUp = oldPosRootNode; }
            Node temp = newPosRootNode.nextNodeDown;
            newPosRootNode.nextNodeDown = oldPosRootNode.nextNodeDown;
            oldPosRootNode.nextNodeDown = temp;

            //päivitä dictionary refs
            nodes.Remove(oldRootPos);
            nodes.Remove(newRootPos);
            nodes.Add(newRootPos, oldPosRootNode);
            nodes.Add(oldRootPos, newPosRootNode);

            //siirrä loppupino
            int stackHeight = 1;
            Node nextUp = oldPosRootNode;
            while (nextUp.isOccupied) {
                nextUp = nextUp.nextNodeUp;
                Vector3 newPos = AddHeight(newRootPos, stackHeight);
                Debug.Log("moving node from: " + nextUp.transform.position + " to: " + newPos);
                nodes.Remove(nextUp.transform.position);
                nextUp.MoveToNewPos(newPos);
                nodes.Add(newPos, nextUp);
                stackHeight++;
            }
        }
    }

    private Vector3 AddHeight(Vector3 pos, int amount) {
        return new Vector3(pos.x, pos.y + (amount * nodeInterval), pos.z);
    }

    //only the topmost building is movable/removable
    public bool IsNodeRemoveable(Vector3 pos) {
        Node node = getNodeByPosition(pos);
        if (node.nextNodeUp == null) { return false; }
        if (node.nextNodeUp.isOccupied) { return false; }
        return true;
    }

    private bool IsNodeOccupied(Vector3 pos) {
        Node node = getNodeByPosition(pos);
        if (node != null) {
            return node.isOccupied;
        }
        Debug.LogError("Node not found");
        return false;
    }

    //decides if "newStructure" can be built in "node", based on the rules written in "stackOrder" dictionary
    private bool IsNodeCompatible(Node node, StructureType newStructure) {
        StructureType lowerStructure = node.structureType;
        List<StructureType> compatibleStructures;
        stackOrder.TryGetValue(lowerStructure, out compatibleStructures);
        if (compatibleStructures.Contains(newStructure)) {
            return true;
        }
        else { return false; }
    }

    //returns the highest occupied node
    private Node GetTopmostStructure(Vector3 pos) {
        Node node = getNodeByPosition(pos);
        while (node.nextNodeUp.isOccupied) {
            node = node.nextNodeUp;
        }
        return node;
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