using UnityEngine;
using System.Collections;

public class GridComponent : MonoBehaviour {
    public StructureType type;
    public Node node;
    public Grid grid;

    private MeshRenderer meshRenderer;
    public Material defaultMat;
    public Material highlightMat;

    public void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        //defaultMat = (Material)Resources.Load("testiatlas", typeof(Material));
        //highlightMat = (Material)Resources.Load("StructureHighlight", typeof(Material));
    }

    public void HighLight() {
        if (meshRenderer.material != highlightMat) { meshRenderer.material = highlightMat; }
    }

    public void UnHighLight() {
        if (meshRenderer.material != defaultMat) { meshRenderer.material = defaultMat; }
    }

    private LayerMask groundLayerMask = 1 << 11;
    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
            grid.RequestNewPosition(node, hit.point);
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
        grid.HighLightValidNodes(node);
        grid.HighlightStack(node);
        AudioManager.Instance.PlayOnce(AudioManager.Instance.buildingPickedUp);
    }

    public void CancelMove() {
        if (registeredToTapEvent) {
            InputManager.OnTap -= OnTap;
            registeredToTapEvent = false;
        }
        if (!transform.position.Equals(posBeforeMove)) {
            grid.MoveStack(node, posBeforeMove);
        }
        grid.UnHighlightNodes();
        grid.UnHighlightStack(node);
    }

    public void FinishMove() {
        if (registeredToTapEvent) { InputManager.OnTap -= OnTap; }
        registeredToTapEvent = false;
        grid.UnHighlightNodes();
        grid.UnHighlightStack(node);
        AudioManager.Instance.PlayOnce(AudioManager.Instance.buildingPlanted);
    }

    public void AttachToGrid(Grid grid) {
        this.grid = grid;
        grid.AttachComponent(this);
    }

    //reattach to grid after recreating from savefile
    public void ReAttachToGrid(Grid grid) {
        this.grid = grid;
        grid.ReAttachComponent(this);
    }

    public void DetachFromGrid() {
        if (node != null) { grid.DetachComponent(node); }
    }

    public void SetNode(Node node) {
        this.node = node;
    }

    public void Replace(GridComponent sub) {
        node.AttachComponent(sub);
        node = null;
        sub.grid = grid;
        if (registeredToTapEvent) {
            InputManager.OnTap -= OnTap;
            registeredToTapEvent = false;
            sub.Move();
        }
    }

    public bool CanBeBuilt(Grid grid) {
        return grid.HasRoomForComponent(this);
    }

    //only the topmost building is removable
    public bool CanBeRemoved() {
        return !node.upperNode.hasComponent;
    }
}
