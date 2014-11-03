using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Singleton Resource manager that persists between scenes 
 * 
 */
public class ResourceManager : Singleton<ResourceManager> {
    public GameObject worker;
    public GUIText woodCountDisplay;
    public GUIText foodCountDisplay;
    public GUIText freeWorkerCountDisplay;
    public GUIText currencyCountDisplay;
    public Resource initialResourceCapacity;
    public Resource initialResourceCount;
    public int initialWorkerCount;
    private Resource resourceCount;
    private Resource resourceCapacity;
    private int workerCount;
    private List<GameObject> freeWorkers;

    public override void Awake() {
        base.Awake();
        resourceCapacity = initialResourceCapacity;
        resourceCount = initialResourceCount;
        workerCount = initialWorkerCount;
        freeWorkers = new List<GameObject>(initialWorkerCount);
        updateGUITexts();
    }

    public void Start() {
        for (int i = 0; i < initialWorkerCount; i++) {
            freeWorkers.Add((GameObject)Instantiate(worker, new Vector3(10, 0, 0), Quaternion.identity));
        }
    }

    public GameObject RequestWorker(GameObject requestingStructure) {
        if (freeWorkers.Count == 0) { return null; }
        GameObject worker = freeWorkers[0];
        freeWorkers.RemoveAt(0);
        worker.GetComponent<Worker>().AssignToStructure(requestingStructure);
        updateGUITexts();
        return worker;
    }

    public void ReleaseWorker(GameObject worker) {
        worker.GetComponent<Worker>().Free();
        freeWorkers.Add(worker);
        updateGUITexts();
    }

    public void AddResources(Resource amount) {
        resourceCount += amount;
        if (resourceCount.wood > resourceCapacity.wood) { resourceCount.wood = resourceCapacity.wood; }
        if (resourceCount.food > resourceCapacity.food) { resourceCount.food = resourceCapacity.food; }
        updateGUITexts();
    }

    public void PayResources(Resource cost) {
        resourceCount -= cost;
        //if (resourceCount.wood < 0) { resourceCount.wood = 0; }
        //if (resourceCount.food < 0) { resourceCount.food = 0; }
        updateGUITexts();
    }

    public void AddWorkers(int amount) {
        for (int i = 0; i < amount; i++) {
            freeWorkers.Add((GameObject)Instantiate(worker, new Vector3(10, 0, 0), Quaternion.identity));
        }
        updateGUITexts();
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

        updateGUITexts();
    }

    public bool CanAffordResources(Resource cost) {
        return resourceCount >= cost;
    }

    public bool HasFreeWorkers(int amount) {
        return freeWorkers.Count >= amount;
    }

    private void updateGUITexts() {
        woodCountDisplay.text = "Wood " + resourceCount.wood.ToString();
        foodCountDisplay.text = "Food " + resourceCount.food.ToString();
        freeWorkerCountDisplay.text = "Workers " + freeWorkers.Count.ToString();
        currencyCountDisplay.text = "Currency " + resourceCount.currency.ToString();
    }
}