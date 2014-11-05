using UnityEngine;
using System.Collections;

public class Node:MonoBehaviour {
    public bool isOccupied;
    public GameObject structure;
    public StructureType structureType;
    public Node nextNodeUp;
    public Node nextNodeDown;
    private Transform myTransform;
    private MeshRenderer meshRenderer;

    public void Awake(){
        myTransform = transform;
        myTransform.parent = Grid.Instance.transform;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Destroy() {
        Destroy(gameObject);
        //UnityEngine.GameObject.DestroyImmediate(renderer.material);
        //UnityEngine.GameObject.Destroy(highlight);
    }

    public void HighLight() {
        meshRenderer.enabled = true;
    }

    public void HideHighLight() {
        meshRenderer.enabled = false;
    }

    public void MoveToNewPos(Vector3 newPos) {
        myTransform.position = newPos;
        if (structure != null) { structure.transform.position = newPos; }
    }

    public void AttachStructure(GameObject structure) {
        this.structure = structure;
        structure.GetComponent<BaseStructure>().AttachToNode(this);
    }

    public void DetachStructure() {
        structure.GetComponent<BaseStructure>().AttachToNode(null);
        structure = null;
    }

    public void SetStructure(GameObject newStructure) {
        structure = newStructure;
    }
}
