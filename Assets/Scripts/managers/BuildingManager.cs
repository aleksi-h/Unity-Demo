using UnityEngine;
using System.Collections;

public class BuildingManager : Singleton<BuildingManager> {
    public GameObject hut;
    public GameObject storage;
    public GameObject sawmill;
    public GameObject field;
    public GameObject statue;

    void Start() {
        GameObject obj = (GameObject)Instantiate(statue, new Vector3(-10, 0, 0), Quaternion.identity);
        Grid.Instance.BuildToNode(obj.transform.position, obj);
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


    public void BuildStructure(GameObject prefab, Resource cost) {
        ResourceManager.Instance.PayResources(cost);
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject newStructure = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        BaseStructure newStructureBase = newStructure.GetComponent<BaseStructure>();
        newStructureBase.Build();
        Grid.Instance.startMove(newStructure);
        //newStructureBase.Move();
        GUIManager.Instance.ShowPlacementGUI(newStructure, newStructureBase.Type);
    }


    public bool CanRemoveStructure(GameObject structure) {
        if (!Grid.Instance.IsNodeRemoveable(structure.transform.position)) {
            return false;
        }
        if (!structure.GetInterface<IRemovable>().RemovalAllowed()) {
            return false;
        }
        return true;
    }

    public void RemoveStructure(GameObject structure) {
        IRemovable removable = structure.GetInterface<IRemovable>();
        removable.Remove();
        Grid.Instance.RemoveFromNode(structure.transform.position);
    }


    public bool CanUpgradeStructure(IUpgradeable upgradeable) {
        if (!upgradeable.UpgradeAllowed()) { return false; }
        if (upgradeable.NextLevelPrefab == null) { return false; }

        Resource upgradeCost = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>().cost;
        if (!ResourceManager.Instance.CanAffordResources(upgradeCost)) { return false; }

        return true;
    }

    public void UpgradeStructure(GameObject structure) {
        IUpgradeable upgradeable = structure.GetInterface<IUpgradeable>();
        Resource upgradeCost = upgradeable.NextLevelPrefab.GetComponent<BaseStructure>().cost;
        ResourceManager.Instance.PayResources(upgradeCost);
        upgradeable.Upgrade();
    }
}
