using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    public bool hasComponent;
    public GridComponent component;
    public Node upperNode;
    public Node lowerNode;
    private Transform myTransform;
    private MeshRenderer meshRenderer;

    public void Awake() {//TODO tallenna transform.position arvo muuttujaan
        myTransform = transform;
        meshRenderer = GetComponent<MeshRenderer>();
        UnHighLight();

        Object test = new Object();
    }

    public void Destroy() {
        if (component != null) { component.SetNode(null); }
        Destroy(gameObject);
    }

    public Node GetTopOfStack() {
        Node node = this;
        while (node.upperNode.hasComponent) {
            node = node.upperNode;
        }
        return node;
    }

    public bool IsInSameStack(Node node) {
        if (myTransform.position.x != node.transform.position.x) { return false; }
        if (myTransform.position.z != node.transform.position.z) { return false; }
        return true;
    }

    public void HighLight() {
        meshRenderer.enabled = true;
    }

    public void UnHighLight() {
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
        hasComponent = true;
    }

    public void DetachComponent() {
        component.SetNode(null);
        component = null;
        hasComponent = false;
    }

    public void setLowerNode(Node node) {
        lowerNode = node;
    }

    public void setUpperNode(Node node) {
        upperNode = node;
    }
}
