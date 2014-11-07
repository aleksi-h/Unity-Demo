using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    public bool isOccupied;
    public GridComponent component;
    public Node upperNode;
    public Node lowerNode;
    private Transform myTransform;
    private MeshRenderer meshRenderer;

    public void Awake() {//TODO tallenna transform.position arvo muuttujaan
        myTransform = transform;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Destroy() {
        if (component != null) { component.SetNode(null); }
        Destroy(gameObject);
    }

    public Node GetTopOfStack() {
        Node node = this;
        while (node.upperNode.isOccupied) { node = node.upperNode; }
        return node;
    }

    public void HighLight() {
        meshRenderer.enabled = true;
    }

    public void HideHighLight() {
        meshRenderer.enabled = false;
    }

    public void MoveToNewPos(Vector3 newPos) {
        myTransform.position = newPos;
        if (component != null) { component.transform.position = newPos; }
    }

    public void MoveUp(float amount) {
        Vector3 newPos = new Vector3(myTransform.position.x, myTransform.position.y + amount, myTransform.position.z);
        myTransform.position = newPos;
        if (component != null) { component.transform.position = newPos; }
    }

    public void AttachComponent(GridComponent component) {
        this.component = component;
        component.SetNode(this);
        isOccupied = true;
    }

    public void DetachComponent() {
        component.SetNode(null);
        component = null;
        isOccupied = false;
    }

    public void setLowerNode(Node node) {
        lowerNode = node;
    }

    public void setUpperNode(Node node) {
        upperNode = node;
    }
}
