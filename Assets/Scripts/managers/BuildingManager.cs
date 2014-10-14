using UnityEngine;
using System.Collections;

public class BuildingManager : Singleton<BuildingManager> {
    public GameObject hut;
    public GameObject storage;
    public GameObject sawmill;
    public GameObject field;
    public GameObject builder;

    private LayerMask structureLayerMask = 1 << 10;
    private LayerMask groundLayerMask = 1 << 11;

    private bool moving;
    private bool newStructure;
    private bool structureSelected;
    private int structureIndex;
    private GameObject selectedStructure;
    private StructureType selectedType;
    private GameObject currentBuilder;

    void Start() {
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

#if UNITY_EDITOR
            //to make testing easier on a pc
            if (Input.GetMouseButtonDown(1)) {
                ConfirmPosition();
            }
#endif
        }
    }

    public void BuildStructure(GameObject obj) {
        BaseStructure structure = obj.GetComponent<BaseStructure>();
        if (ResourceManager.Instance.CanAfford(structure.cost)) {
            ResourceManager.Instance.RemoveResources(structure.cost);
            Vector3 pos = new Vector3(0, 0, 0);
            selectedStructure = (GameObject)Instantiate(obj, pos, Quaternion.identity);
            selectedType = selectedStructure.GetComponent<BaseStructure>().Type;
            moving = true;
            newStructure = true;
            GUIManager.Instance.ShowPlacementGUI(selectedType);
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
        if (newStructure) {
            currentBuilder = (GameObject)Instantiate(builder, selectedStructure.transform.position, Quaternion.identity);
            int duration = selectedStructure.GetComponent<BaseStructure>().buildTime;
            currentBuilder.GetComponent<Builder>().BuildStructure(selectedStructure, duration);
            newStructure = false;
        }
        GUIManager.Instance.ShowDefaultGUI();
    }

    public void UpgradeStructure() {
        IUpgradeable upgradeable = selectedStructure.GetInterface<IUpgradeable>();
        if (upgradeable != null && upgradeable.UpgradeAllowed() && upgradeable.NextLevelPrefab != null) {
            BaseStructure nextLevel = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>();
            Resource upgradeCost = nextLevel.cost;
            if (ResourceManager.Instance.CanAfford(upgradeCost)) {
                ResourceManager.Instance.RemoveResources(upgradeCost);
                int duration = nextLevel.buildTime;
                GameObject nextLevelPrefab = upgradeable.NextLevelPrefab;
                upgradeable.PrepareForUpgrade();
                currentBuilder = (GameObject)Instantiate(builder, selectedStructure.transform.position, Quaternion.identity);
                currentBuilder.GetComponent<Builder>().UpgradeStructure(selectedStructure, duration, nextLevelPrefab);
                GUIManager.Instance.ShowDefaultGUI();
            }
        }
    }
}
