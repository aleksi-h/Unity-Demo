using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


public class BuildingManager : Singleton<BuildingManager> {
    public Grid grid;
    public GameObject[] hut;
    public GameObject[] storage;
    public GameObject[] sawmill;
    public GameObject[] field;
    public GameObject[] statue;
    public GameObject[] outpost;

    public override void Awake() {
        base.Awake();
        SaveLoad.SaveState += SaveState;
        SaveLoad.LoadState += LoadState;
        SaveLoad.InitGame += FirstLaunch;
    }

    public bool CanBuildStructure(GameObject prefab, Resource cost) {
        if (!prefab.GetComponent<GridComponent>().CanBeBuilt(grid)) { return false; }
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
        newStructure.transform.parent = transform;
        newStructure.GetComponent<BaseStructure>().Build(grid);
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
        obj.transform.parent = transform;
        obj.GetComponent<GridComponent>().AttachToGrid(grid);
        obj.GetComponent<BaseStructure>().Activate();

        obj = (GameObject)Instantiate(outpost[0], new Vector3(0, 0, 0), Quaternion.identity);
        obj.transform.parent = transform;
        obj.GetComponent<GridComponent>().AttachToGrid(grid);
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
                BaseStructure bs = obj.GetComponent<BaseStructure>();
                UpgradableStructure upgradable = obj.GetComponent<UpgradableStructure>();
                s.level = bs.level;
                if (bs.isUnderConstruction) {
                    s.isUnderConstruction = true;
                    s.processTimeLeft = bs.processTimeLeft;
                }
                else if (upgradable != null) {
                    s.isUpgrading = upgradable.isUpgrading;
                    s.processTimeLeft = bs.processTimeLeft;
                }
                s.type = obj.GetComponent<GridComponent>().type;
                if (obj.ImplementsInterface<IEmployer>()) { s.workerCount = obj.GetInterface<IEmployer>().Workers.Count; }
                myState.structures.Add(s);
            }
        }
        gamestate.buildingManagerState = myState;
    }

    private void LoadState(SaveLoad.GameState gamestate) {
        GameObject obj = null;
        foreach (BMState.StructureRepresentation s in gamestate.buildingManagerState.structures) {
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
                case StructureType.Outpost:
                    obj = (GameObject)Instantiate(outpost[s.level - 1], s.GetPos(), Quaternion.identity);
                    break;
                default:
                    break;
            }
            if (obj != null) {
                obj.transform.parent = transform;
                obj.GetComponent<GridComponent>().ReAttachToGrid(grid);
                if (obj.ImplementsInterface<IEmployer>()) { obj.GetInterface<IEmployer>().LoadWorkers(s.workerCount); }

                if (s.isUnderConstruction) { obj.GetComponent<BaseStructure>().ContinueBuild(s.processTimeLeft); }
                else if (s.isUpgrading) { obj.GetInterface<IUpgradeable>().ContinueUpgrade(s.processTimeLeft); }
                else { obj.GetComponent<BaseStructure>().Activate(); }
            }
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
            [OptionalField(VersionAdded = 2)]
            public bool isUnderConstruction;
            [OptionalField(VersionAdded = 2)]
            public float processTimeLeft;
            [OptionalField(VersionAdded = 3)]
            public bool isUpgrading;
            public StructureType type;
            public int workerCount;
            public StructureRepresentation() {
                this.tag = "";
                this.pos = new float[] { 0, 0, 0 };
                this.rot = new float[] { 0, 0, 0 };
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