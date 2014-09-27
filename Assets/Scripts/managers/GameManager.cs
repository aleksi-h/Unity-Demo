using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] structures;

    private LayerMask placementLayerMask = 1 << 9;
    private LayerMask structureLayerMask = 1 << 10;
    private LayerMask groundLayerMask = 1 << 11;

    private enum State { Idle, Building, Moving }
    private State state;

    private bool structureSelected;
    private int structureIndex;
    private GameObject selectedStructure;
    private GameObject selectedSquare;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            switch (state)
            {
                case State.Idle:
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask))
                        {
                            selectedStructure = hit.collider.gameObject;
                            /*
                            Vector3 down = transform.TransformDirection(Vector3.down);
                            Vector3 position = selectedStructure.transform.position;
                            Vector3 elevatedPosition = new Vector3(position.x, position.y + 1, position.z);
                            if (Physics.Raycast(elevatedPosition, down, out hit, 10, placementLayerMask))
                            {selectedSquare = hit.collider.gameObject;}
                             */
                            structureSelected = true;
                            GUIManager.Instance.ShowStructureGUI();
                        }
                    }
                    break;
                case State.Moving:
                        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask))
                        {
                            //it's important to propose only grounded positions to GetNearestFreeNode
                            float ground = 0;
                            selectedStructure.transform.position = Grid.Instance.GetNearestFreeNode(hit.point.x, ground, hit.point.z);
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            ConfirmBuild();
                        }
                    break;
        }
    }

    public void BuildStructure(int index)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        selectedStructure = (GameObject)Instantiate(structures[index], pos, Quaternion.identity);
        state = State.Moving;
        GUIManager.Instance.ShowPlacementGUI();
    }

    public void RemoveSelection()
    {
        state = State.Idle;
        structureSelected = false;
    }

    public void DeleteStructure()
    {
        if (structureSelected && Grid.Instance.IsBuildingTopMost(selectedStructure.transform.position))
        {
            IRemovable removable = selectedStructure.GetInterface<IRemovable>();
            if (removable != null)
            {
                removable.Remove();
                Grid.Instance.RemoveFromNode(selectedStructure.transform.position);
                structureSelected = false;
                GUIManager.Instance.ShowDefaultGUI();
            }
        }
    }

    public void MoveStructure()
    {
        if (Grid.Instance.IsBuildingTopMost(selectedStructure.transform.position))
        {
            Grid.Instance.RemoveFromNode(selectedStructure.transform.position);
            state = State.Moving;
            GUIManager.Instance.ShowPlacementGUI();
        }
    }

    public void ConfirmBuild()
    {
        Grid.Instance.BuildToNode(selectedStructure.transform.position);
        state = State.Idle;
        GUIManager.Instance.ShowDefaultGUI();
    }
}
