using UnityEngine;
using System.Collections;

public class BuildingManager : Singleton<BuildingManager> {
    public GameObject hut;
    public GameObject storage;
    public GameObject sawmill;
    public GameObject field;
    public GameObject statue;

    private LayerMask structureLayerMask = 1 << 10;
    private LayerMask groundLayerMask = 1 << 11;

    private bool moving;
    private bool newStructure;
    private bool structureSelected;
    private int structureIndex;
    private GameObject selectedStructure;
    private StructureType selectedType;

    void Start() {
        GameObject obj = (GameObject)Instantiate(statue, new Vector3(-10, 0, 0), Quaternion.identity);
        Grid.Instance.BuildToNode(obj.transform.position, selectedType);
        obj.GetComponent<BaseStructure>().Activate();
        InputManager.OnTap += OnTap;
    }

    private void OnTap(Vector3 tapPos) {
        if (!moving) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
                selectedStructure = hit.collider.gameObject;
                selectedType = selectedStructure.GetComponent<BaseStructure>().Type;
                structureSelected = true;
                GUIManager.Instance.ShowStructureGUI();
            }
        }
        else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
                Vector3 currentPosition = selectedStructure.transform.position;
                selectedStructure.transform.position = Grid.Instance.GetNearestValidNode(currentPosition, hit.point, selectedType);
            }
        }
    }

    public void BuildStructure(GameObject obj) {
        bool canAffordStructure = true;
        BaseStructure structure = obj.GetComponent<BaseStructure>();
        if (!ResourceManager.Instance.CanAffordResources(structure.cost)) {
            canAffordStructure = false;
        }
        if (obj.ImplementsInterface<IEmployer>()) {
            if (!ResourceManager.Instance.HasFreeWorkers(obj.GetInterface<IEmployer>().MinWorkerCount)) {
                canAffordStructure = false;
            }
        }
        if (canAffordStructure) {
            ResourceManager.Instance.PayResources(structure.cost);
            Vector3 pos = new Vector3(0, 0, 0);
            selectedStructure = (GameObject)Instantiate(obj, pos, Quaternion.identity);
            selectedType = selectedStructure.GetComponent<BaseStructure>().Type;
            selectedStructure.GetComponent<BaseStructure>().Build();
            GUIManager.Instance.ShowPlacementGUI(selectedType);
            moving = true;
        }
    }

    public void DeleteStructure() {
        if (structureSelected && Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position)) {
            IRemovable removable = selectedStructure.GetInterface<IRemovable>();
            if (removable != null && removable.RemovalAllowed()) {
                removable.Remove();
                Grid.Instance.RemoveFromNode(selectedStructure.transform.position);
                structureSelected = false;
                GUIManager.Instance.ShowDefaultGUI();
            }
        }
    }

    public void MoveStructure() {
        if (Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position)) {
            Grid.Instance.RemoveFromNode(selectedStructure.transform.position);
            moving = true;
            GUIManager.Instance.ShowPlacementGUI(selectedType);
        }
    }

    public void ConfirmPosition() {
        moving = false;
        Grid.Instance.BuildToNode(selectedStructure.transform.position, selectedType);
        GUIManager.Instance.ShowDefaultGUI();
    }

    public void UpgradeStructure() {
        IUpgradeable upgradeable = selectedStructure.GetInterface<IUpgradeable>();
        if (upgradeable != null && upgradeable.UpgradeAllowed() && upgradeable.NextLevelPrefab != null) {
            Resource upgradeCost = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>().cost;
            if (ResourceManager.Instance.CanAffordResources(upgradeCost)) {
                ResourceManager.Instance.PayResources(upgradeCost);
                upgradeable.Upgrade();
                GUIManager.Instance.ShowDefaultGUI();
            }
        }
    }
}
