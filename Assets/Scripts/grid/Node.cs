using UnityEngine;
using System.Collections;

public class Node
{
    public bool occupied;
    public StructureType structureType;
    public Vector3 position;
    public Node nextNodeUp;
    public Node nextNodeDown;
    private GameObject obj;
    MeshRenderer renderer;

    public Node(Vector3 pos, bool occupied)
    {
        position = pos;
        this.occupied = occupied;

        CreateMesh();
    }

    public void HighLight()
    {
        renderer.enabled = true;   
    }

    public void HideHighLight()
    {
        renderer.enabled = false;
    }

    private void CreateMesh()
    {
        obj = new GameObject("node");
        obj.transform.parent = Grid.Instance.transform;
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        renderer = obj.AddComponent<MeshRenderer>();
        //renderer.material = new Material(Shader.Find("Custom/UnlitColor"));
        //renderer.material.SetColor("_Color", new Color(0.5f, 0.72f, 1f, 1f));
        renderer.material = new Material(Shader.Find("Unlit/Texture"));

        float radius = 1.8f;
        float height = 0.02f;
        Vector3 p0 = new Vector3(-radius, 0, -radius) + position;
        Vector3 p1 = new Vector3(-radius, height, -radius) + position;
        Vector3 p2 = new Vector3(radius, height, -radius) + position;
        Vector3 p3 = new Vector3(radius, 0, -radius) + position;

        Vector3 p4 = new Vector3(-radius, 0, radius) + position;
        Vector3 p5 = new Vector3(-radius, height, radius) + position;
        Vector3 p6 = new Vector3(radius, height, radius) + position;
        Vector3 p7 = new Vector3(radius, 0, radius) + position;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        mesh.triangles = new int[]{
        0,1,2,
        0,2,3,
        4,5,1,
        4,1,0,
        5,6,2,
        5,2,1,
        3,2,6,
        3,6,7,
        7,4,0,
        7,0,3,
        7,6,5,
        7,5,4
};

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}
