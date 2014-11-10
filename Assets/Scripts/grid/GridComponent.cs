﻿using UnityEngine;
using System.Collections;

public class GridComponent : MonoBehaviour {
    public StructureType type;
    public Node node;


    private LayerMask groundLayerMask = 1 << 11;
    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
            Grid.Instance.RequestNewPosition(node, hit.point);
        }
    }

    private Vector3 posBeforeMove;
    private bool registeredToTapEvent;
    public void Move() {
        posBeforeMove = transform.position;
        if (!registeredToTapEvent) {
            InputManager.OnTap += OnTap;
            registeredToTapEvent = true;
        }
        GUIManager.Instance.ShowPlacementGUI(gameObject);
        Grid.Instance.HighLightValidNodes(node);
    }

    public void CancelMove() {
        if (registeredToTapEvent) {
            InputManager.OnTap -= OnTap;
            registeredToTapEvent = false;
        }
        if (!transform.position.Equals(posBeforeMove)) {
            Grid.Instance.MoveStack(node, posBeforeMove);
        }
        Grid.Instance.HideHighlight();
    }

    public void FinishMove() {
        if (registeredToTapEvent) { InputManager.OnTap -= OnTap; }
        registeredToTapEvent = false;
        Grid.Instance.HideHighlight();
    }


    public void AttachToGrid() {
        Grid.Instance.AttachComponent(this);
    }

    public void DetachFromGrid() {
        if (node != null) { Grid.Instance.DetachComponent(node); }
    }

    public void SetNode(Node node) {
        this.node = node;
    }

    public void Replace(GridComponent sub) {
        node.AttachComponent(sub);
        node = null;
    }

    public bool CanBeBuilt() {
        return Grid.Instance.HasRoom(this);
    }

    //only the topmost building is removable
    public bool CanBeRemoved() {
        //if (node.upperNode == null) { return false; }
        if (node.upperNode.isOccupied) { return false; }
        return true;
    }
}
