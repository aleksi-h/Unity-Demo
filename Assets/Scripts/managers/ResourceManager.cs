using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Singleton Resource manager that persists between scenes 
 * 
 */
public class ResourceManager : Singleton<ResourceManager> {
    public GameObject worker;
    public Resource initialResourceCapacity;
    public Resource initialResourceCount;
    public int initialWorkerCount;
    private Resource resourceCount;
    private Resource resourceCapacity;
    private List<GameObject> freeWorkers;
    private RMState state;
    private Transform myTransform;

    public override void Awake() {
        base.Awake();
        SaveLoad.SaveState += SaveState;
        SaveLoad.LoadState += LoadState;
        SaveLoad.InitGame += FirstLaunch;

        myTransform = transform;
        resourceCapacity = new Resource(0, 0, 0, 0);
        resourceCount = new Resource(0, 0, 0, 0);
        freeWorkers = new List<GameObject>(initialWorkerCount);
        updateResourceTexts();
    }

    public void Start() {
        
    }

    //create a worker directly into a building. employing buildings call this method when the game loads from savefile
    public GameObject CreateWorker(GameObject requestingStructure) {
        GameObject newWorker = (GameObject)Instantiate(worker, requestingStructure.transform.position, Quaternion.identity);
        newWorker.transform.parent = myTransform;
        newWorker.GetComponent<Worker>().AssignToStructure(requestingStructure);
        return newWorker;
    }

    public GameObject RequestWorker(GameObject requestingStructure) {
        if (freeWorkers.Count == 0) { return null; }
        GameObject worker = freeWorkers[0];
        freeWorkers.RemoveAt(0);
        worker.GetComponent<Worker>().AssignToStructure(requestingStructure);
        updateResourceTexts();
        return worker;
    }

    public void ReleaseWorker(GameObject worker) {
        worker.GetComponent<Worker>().Free();
        freeWorkers.Add(worker);
        updateResourceTexts();
    }

    public void AddResources(Resource amount) {
        resourceCount += amount;
        if (resourceCount.wood > resourceCapacity.wood) { resourceCount.wood = resourceCapacity.wood; }
        if (resourceCount.food > resourceCapacity.food) { resourceCount.food = resourceCapacity.food; }
        updateResourceTexts();
    }

    public void PayResources(Resource cost) {
        resourceCount -= cost;
        //if (resourceCount.wood < 0) { resourceCount.wood = 0; }
        //if (resourceCount.food < 0) { resourceCount.food = 0; }
        updateResourceTexts();
    }

    public void AddWorkers(int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject newWorker = (GameObject)Instantiate(worker, new Vector3(10, 0, 0), Quaternion.identity);
            newWorker.transform.parent = myTransform;
            freeWorkers.Add(newWorker);
        }
        updateResourceTexts();
    }

    public void IncreaseResourceCapacity(Resource addedCapacity) {
        resourceCapacity += addedCapacity;
    }

    public void DecreaseResourceCapacity(Resource removedCapacity) {
        resourceCapacity -= removedCapacity;

        //if (resourceCapacity.wood < 0) { resourceCapacity.wood = 0; }
        if (resourceCount.wood > resourceCapacity.wood) { resourceCount.wood = resourceCapacity.wood; }

        //if (resourceCapacity.food < 0) { resourceCapacity.food = 0; }
        if (resourceCount.food > resourceCapacity.food) { resourceCount.food = resourceCapacity.food; }

        updateResourceTexts();
    }

    public bool CanAffordResources(Resource cost) {
        return resourceCount >= cost;
    }

    public bool HasFreeWorkers(int amount) {
        return freeWorkers.Count >= amount;
    }

    private void updateResourceTexts() {
        GUIManager.Instance.RefreshResourceTexts(resourceCount, freeWorkers.Count);
    }

    private void FirstLaunch() {
        resourceCapacity = initialResourceCapacity;
        resourceCount = initialResourceCount;
        for (int i = 0; i < initialWorkerCount; i++) {
            GameObject newWorker = (GameObject)Instantiate(worker, new Vector3(10, 0, 0), Quaternion.identity);
            newWorker.transform.parent = myTransform;
            freeWorkers.Add(newWorker);
        }
    }


    private void SaveState(SaveLoad.GameState gamestate) {
        RMState myState = new RMState();
        myState.resourceCapacity = resourceCapacity;
        myState.resourceCount = resourceCount;
        myState.freeWorkerCount = freeWorkers.Count;
        gamestate.resourceManagerState = myState;
    }

    private void LoadState(SaveLoad.GameState gamestate) {
        resourceCapacity = gamestate.resourceManagerState.resourceCapacity;
        resourceCount = gamestate.resourceManagerState.resourceCount;

        for (int i = 0; i < gamestate.resourceManagerState.freeWorkerCount; i++) {
            GameObject newWorker = (GameObject)Instantiate(worker, new Vector3(10, 0, 0), Quaternion.identity);
            newWorker.transform.parent = myTransform;
            freeWorkers.Add(newWorker);
        }
        updateResourceTexts();
    }

    [System.Serializable]
    public class RMState {
        public Resource resourceCount;
        public Resource resourceCapacity;
        public int freeWorkerCount;
    }
}