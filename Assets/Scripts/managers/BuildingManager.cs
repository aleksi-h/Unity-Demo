using UnityEngine;
using System.Collections;

public class BuildingManager : Singleton<BuildingManager>
{
    public GameObject hut;
    public GameObject storage;
    public GameObject sawmill;
    public GameObject field;

    private LayerMask structureLayerMask = 1 << 10;
    private LayerMask groundLayerMask = 1 << 11;

    private enum State { Idle, Moving }
    private State state;

    private bool structureSelected;
    private int structureIndex;
    private GameObject selectedStructure;
    private StructureType selectedType;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (state == State.Idle && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 1100, structureLayerMask))
            {
                selectedStructure = hit.collider.gameObject;
                selectedType = selectedStructure.GetComponent<BaseStructure>().Type;
                structureSelected = true;
                GUIManager.Instance.ShowStructureGUI();
            }
        }
        if (state == State.Moving)
        {
            if (Physics.Raycast(ray, out hit, 1100, groundLayerMask))
            {
                Vector3 currentPosition = selectedStructure.transform.position;
                selectedStructure.transform.position = Grid.Instance.GetNearestValidNode(currentPosition, hit.point, selectedType);
            }

            //to make testing easier on a pc
            if (Input.GetMouseButtonDown(1))
            {
                ConfirmBuild();
            }
        }
    }

    public void BuildStructure(GameObject obj)
    {
        BaseStructure structure = obj.GetComponent<BaseStructure>();
        if (ResourceManager.Instance.CanAfford(structure.cost))
        {
            ResourceManager.Instance.RemoveResources(structure.cost);
            Vector3 pos = new Vector3(0, 0, 0);
            selectedStructure = (GameObject)Instantiate(obj, pos, Quaternion.identity);
            selectedType = selectedStructure.GetComponent<BaseStructure>().Type;
            state = State.Moving;
            GUIManager.Instance.ShowPlacementGUI(selectedType);
        }
    }

    public void RemoveSelection()
    {
        state = State.Idle;
        structureSelected = false;
    }

    public void DeleteStructure()
    {
        if (structureSelected && Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position))
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
        if (Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position))
        {
            Grid.Instance.RemoveFromNode(selectedStructure.transform.position);
            state = State.Moving;
            GUIManager.Instance.ShowPlacementGUI(selectedType);
        }
    }

    public void ConfirmBuild()
    {
        Grid.Instance.BuildToNode(selectedStructure.transform.position, selectedType);
        state = State.Idle;
        GUIManager.Instance.ShowDefaultGUI();
    }

    public void UpgradeStructure()
    {
        IUpgradeable upgradeable = selectedStructure.GetInterface<IUpgradeable>();
        if (upgradeable != null)
        {
            Resource upgradeCost = upgradeable.UpgradeCost;
            if (ResourceManager.Instance.CanAfford(upgradeCost) && upgradeable.NextLevelPrefab != null)
            {
                ResourceManager.Instance.RemoveResources(upgradeCost);
                int duration = upgradeable.UpgradeDuration;
                GameObject nextLevelPrefab = upgradeable.NextLevelPrefab;
                Builder builder = gameObject.AddComponent<Builder>();
                builder.UpgradeStructure(selectedStructure, duration, nextLevelPrefab);
                upgradeable.Upgrade();
                GUIManager.Instance.ShowDefaultGUI();
            }
        }
    }
}
