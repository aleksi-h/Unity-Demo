using UnityEngine;
using System.Collections;

/*
 * Singleton Resource manager that persists between scenes 
 * 
 */
public class ResourceManager : Singleton<ResourceManager> {
    public GUIText woodCountDisplay;
    public GUIText foodCountDisplay;
    public GUIText workerCountDisplay;
    public GUIText currencyCountDisplay;
    public Resource initialResourceCapacity;
    public Resource initialResourceCount;
    public int initialWorkerCount;
    private Resource resourceCount;
    private Resource resourceCapacity;
    private int workerCount;

    public override void Awake() {
        base.Awake();
        resourceCapacity = initialResourceCapacity;
        resourceCount = initialResourceCount;
        workerCount = initialWorkerCount;
        updateGUITexts();
    }

    public void AddResources(Resource amount) {
        resourceCount += amount;
        if (resourceCount.wood > resourceCapacity.wood) { resourceCount.wood = resourceCapacity.wood; }
        if (resourceCount.food > resourceCapacity.food) { resourceCount.food = resourceCapacity.food; }
        updateGUITexts();
    }

    public void PayWithResources(Resource cost) {
        resourceCount -= cost;
        //if (resourceCount.wood < 0) { resourceCount.wood = 0; }
        //if (resourceCount.food < 0) { resourceCount.food = 0; }
        updateGUITexts();
    }

    public void AddWorkers(int amount) {
        workerCount += amount;
        updateGUITexts();
    }

    public void RemoveWorkers(int amount) {
        workerCount -= amount;
        updateGUITexts();
    }

    public void IncreaseResourceCapacity(Resource addedCapacity) {
        resourceCapacity += addedCapacity;
    }

    public void DecreaseResourceCapacity(Resource removedCapacity) {
        resourceCapacity -= removedCapacity;

        if (resourceCapacity.wood < 0) { resourceCapacity.wood = 0; }
        if (resourceCount.wood > resourceCapacity.wood) { resourceCount.wood = resourceCapacity.wood; }

        if (resourceCapacity.food < 0) { resourceCapacity.food = 0; }
        if (resourceCount.food > resourceCapacity.food) { resourceCount.food = resourceCapacity.food; }

        updateGUITexts();
    }

    public bool CanAffordResources(Resource cost) {
        return resourceCount >= cost;
    }

    private void updateGUITexts() {
        woodCountDisplay.text = "Wood " + resourceCount.wood.ToString();
        foodCountDisplay.text = "Food " + resourceCount.food.ToString();
        workerCountDisplay.text = "Workers " + workerCount.ToString();
        currencyCountDisplay.text = "Currency " + resourceCount.currency.ToString();
    }
}