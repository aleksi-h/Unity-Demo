using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] structures;

    private LayerMask placementLayerMask = 1 << 9;
    private LayerMask structureLayerMask = 1 << 10;

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            switch (state)
            {
                case State.Idle:
                    if (Physics.Raycast(ray, out hit, 1100, structureLayerMask))
                    {
                        selectedStructure = hit.collider.gameObject;
                        Vector3 down = transform.TransformDirection(Vector3.down);
                        Vector3 position = selectedStructure.transform.position;
                        Vector3 elevatedPosition = new Vector3(position.x, position.y + 1, position.z);
                        if (Physics.Raycast(elevatedPosition, down, out hit, 10, placementLayerMask))
                        {
                            selectedSquare = hit.collider.gameObject;
                        }
                        structureSelected = true;
                        GUIManager.Instance.ShowStructureGUI();
                    }
                    break;
                case State.Building:
                    if (Physics.Raycast(ray, out hit, 1100, placementLayerMask))
                    {
                        if (hit.collider.gameObject.tag == "square_free")
                        {
                            Instantiate(structures[structureIndex], hit.collider.gameObject.transform.position, Quaternion.identity);
                            hit.collider.gameObject.tag = "square_occupied";
                            state = State.Idle;
                            GUIManager.Instance.ShowBuildGUI();
                        }
                    }
                    break;
                case State.Moving:
                    if (Physics.Raycast(ray, out hit, 1100, placementLayerMask))
                    {
                        if (hit.collider.gameObject.tag == "square_free")
                        {
                            selectedStructure.transform.position = hit.collider.gameObject.transform.position;
                            hit.collider.gameObject.tag = "square_occupied";
                            selectedSquare.tag = "square_free";
                            structureSelected = false;
                            state = State.Idle;
                            GUIManager.Instance.ShowBuildGUI();
                        }
                    }
                    break;
            }
        }
    }

    public void SelectStructure(int index)
    {
        state = State.Building;
        structureIndex = index;
    }

    public void RemoveSelection()
    {
        state = State.Idle;
        structureSelected = false;
    }

    public void DeleteStructure()
    {
        if (structureSelected)
        {
            IRemovable removable = selectedStructure.GetInterface<IRemovable>();
            if (removable != null)
            {
                removable.Remove();
                selectedSquare.tag = "square_free";
                structureSelected = false;
                GUIManager.Instance.ShowBuildGUI();
            }
        }
    }

    public void MoveStructure()
    {
        state = State.Moving;
    }
}
