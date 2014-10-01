using UnityEngine;
using System.Collections;

public class Node {
    public bool occupied;
    public StructureType structureType;
    public Vector3 position;
    public Node nextNodeUp;
    public Node nextNodeDown;

    public Node(Vector3 pos, bool occupied)
    {
        position = pos;
        this.occupied = occupied;
    }
}
