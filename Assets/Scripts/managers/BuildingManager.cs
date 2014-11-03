using UnityEngine;
using System.Collections;

public class BuildingManager : Singleton<BuildingManager> {
    public GameObject hut;
    public GameObject storage;
    public GameObject sawmill;
    public GameObject field;
    public GameObject statue;

    private bool moving;
    private bool newStructure;
    private int structureIndex;
    private StructureType selectedType;

    void Start() {
        GameObject obj = (GameObject)Instantiate(statue, new Vector3(-10, 0, 0), Quaternion.identity);
        Grid.Instance.BuildToNode(obj.transform.position, StructureType.Special);
        obj.GetComponent<BaseStructure>().Activate();
    }

    public bool CanAffordStructure(GameObject prefab, Resource cost) {
        if (!ResourceManager.Instance.CanAffordResources(cost)) { return false; }
        if (prefab.ImplementsInterface<IEmployer>()) {
            int minWorkerCount = prefab.GetInterface<IEmployer>().MinWorkerCount;
            if (!ResourceManager.Instance.HasFreeWorkers(minWorkerCount)) {
                return false;
            }
        }
        return true;
    }

    public bool CanUpgradeStructure(IUpgradeable upgradeable) {
        if (!upgradeable.UpgradeAllowed()) { return false; }
        if (upgradeable.NextLevelPrefab == null) { return false; }

        Resource upgradeCost = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>().cost;
        if (!ResourceManager.Instance.CanAffordResources(upgradeCost)) { return false; }

        return true;
    }

    public void BuildStructure(GameObject prefab, Resource cost) {
        ResourceManager.Instance.PayResources(cost);
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject newStructure = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        BaseStructure newStructureBase = newStructure.GetComponent<BaseStructure>();
        selectedType = newStructureBase.Type;
        newStructureBase.Build();
        newStructureBase.Move();
        GUIManager.Instance.ShowPlacementGUI(newStructure, selectedType);
    }

    public bool IsBuildingRemovable(GameObject structure) {
        if (!Grid.Instance.IsBuildingMoveable(structure.transform.position)) {
            return false;
        }
        if (!structure.GetInterface<IRemovable>().RemovalAllowed()) {
            return false;
        }
        return true;
    }

    public void DeleteStructure(GameObject structure) {
        //if (Grid.Instance.IsBuildingMoveable(structure.transform.position)) {
            IRemovable removable = structure.GetInterface<IRemovable>();
            //if (removable != null && removable.RemovalAllowed()) {
                removable.Remove();
                Grid.Instance.RemoveFromNode(structure.transform.position);
                //GUIManager.Instance.ShowDefaultMenu();
          //  }
        //}
    }

    public void UpgradeStructure(GameObject structure) {
        IUpgradeable upgradeable = structure.GetInterface<IUpgradeable>();
        Resource upgradeCost = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>().cost;
        ResourceManager.Instance.PayResources(upgradeCost);
        upgradeable.Upgrade();
        //GUIManager.Instance.ShowDefaultMenu();
    }
}
