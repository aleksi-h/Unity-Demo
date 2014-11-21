using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingManager : Singleton<BuildingManager> {
    public GameObject[] hut;
    public GameObject[] storage;
    public GameObject[] sawmill;
    public GameObject[] field;
    public GameObject[] statue;

    void Awake() {
        SaveLoad.SaveState += SaveState;
        SaveLoad.LoadState += LoadState;
        SaveLoad.InitGame += FirstLaunch;
    }

    public bool CanBuildStructure(GameObject prefab, Resource cost) {
        if (!prefab.GetComponent<GridComponent>().CanBeBuilt()) { return false; }
        if (!ResourceManager.Instance.CanAffordResources(cost)) { return false; }
        if (prefab.ImplementsInterface<IEmployer>()) {
            int minWorkerCount = prefab.GetInterface<IEmployer>().MinWorkerCount;
            if (!ResourceManager.Instance.HasFreeWorkers(minWorkerCount)) { return false; }
        }
        return true;
    }


    public void BuildStructure(GameObject prefab, Resource cost) {
        ResourceManager.Instance.PayResources(cost);
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject newStructure = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        newStructure.GetComponent<BaseStructure>().Build();
    }


    public bool CanRemoveStructure(GameObject structure) {
        return structure.GetInterface<IRemovable>().RemovalAllowed();
    }

    public void RemoveStructure(GameObject structure) {
        IRemovable removable = structure.GetInterface<IRemovable>();
        removable.Remove();
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

    private void FirstLaunch() {
        GameObject obj = (GameObject)Instantiate(statue[0], new Vector3(0, 0, 0), Quaternion.identity);
        obj.GetComponent<GridComponent>().AttachToGrid();
        obj.GetComponent<BaseStructure>().Activate();
    }



    private void SaveState(SaveLoad.GameState gamestate) {
        BMState myState = new BMState();
        GameObject[] structures = GameObject.FindGameObjectsWithTag("structure");
        if (structures.Length > 0) {
            foreach (GameObject obj in structures) {
                BMState.StructureRepresentation s = new BMState.StructureRepresentation();
                s.tag = obj.tag;
                s.SetPos(obj.transform.position);
                s.SetRot(obj.transform.eulerAngles);
                s.level = obj.GetComponent<BaseStructure>().level;
                s.type = obj.GetComponent<GridComponent>().type;
                if (obj.ImplementsInterface<IEmployer>()) { s.workerCount = obj.GetInterface<IEmployer>().Workers.Count; }
                Debug.Log("saving: " + s.type + " lvl " + s.level);
                myState.structures.Add(s);
            }
        }
        gamestate.buildingManagerState = myState;
    }

    private void LoadState(SaveLoad.GameState gamestate) {
        GameObject obj = null;
        foreach (BMState.StructureRepresentation s in gamestate.buildingManagerState.structures) {
            Debug.Log("loading " + s.type + " lvl " + s.level);
            switch (s.type) {
                case StructureType.Field:
                    obj = (GameObject)Instantiate(field[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                case StructureType.Sawmill:
                    obj = (GameObject)Instantiate(sawmill[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                case StructureType.Hut:
                    obj = (GameObject)Instantiate(hut[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                case StructureType.Storage:
                    obj = (GameObject)Instantiate(storage[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                case StructureType.Statue:
                    obj = (GameObject)Instantiate(statue[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                default:
                    break;
            }
            if (obj != null) {
                if (obj.ImplementsInterface<IEmployer>()) { obj.GetInterface<IEmployer>().LoadWorkers(s.workerCount); }
                obj.GetComponent<GridComponent>().ReAttachToGrid();
                obj.GetComponent<BaseStructure>().Activate();
            }
            else { Debug.LogError("Error loading buildings from savefile. " + s.tag + " bad tag"); }
        }
    }

    [System.Serializable]
    public class BMState {
        public List<StructureRepresentation> structures;
        public BMState() {
            structures = new List<StructureRepresentation>();
        }

        [System.Serializable]
        public class StructureRepresentation {
            public string tag;
            public float[] pos;
            public float[] rot;
            public int level;
            public StructureType type;
            public int workerCount;
            public StructureRepresentation() {
                this.tag = "";
                this.pos = new float[] { 0,0,0 };
                this.rot = new float[] { 0,0,0};
            }
            public void SetPos(Vector3 pos) {
                this.pos = new float[] { pos.x, pos.y, pos.z };
            }
            public void SetRot(Vector3 rot) {
                this.rot = new float[] { rot.x, rot.y, rot.z };
            }
            public Vector3 GetPos() {
                return new Vector3(pos[0], pos[1], pos[2]);
            }
            public Vector3 GetRot() {
                return new Vector3(rot[0], rot[1], rot[2]);
            }
        }
    }
}