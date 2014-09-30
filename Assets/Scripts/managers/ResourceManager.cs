using UnityEngine;
using System.Collections;

/*
 * A singleton Resource manager that persists between scenes 
 * 
 */
public class ResourceManager : Singleton<ResourceManager>
{    
    public GUIText woodCountDisplay;
    public GUIText foodCountDisplay;
    public GUIText workerCountDisplay;
    private int woodCount;
    private int woodCapacity;
    private int foodCount;
    private int foodCapacity;
    private int workerCount;
   
    void Start()
    {
        woodCapacity = 0;
        foodCapacity = 0;
        updateGUITexts();
    }

    public void AddWood(int amount)
    {
        woodCount += amount;
        if (woodCount > woodCapacity)
        {
            woodCount = woodCapacity;
        }
        updateGUITexts();
    }

    public void AddFood(int amount)
    {
        foodCount += amount;
        if (foodCount > foodCapacity)
        {
            foodCount = foodCapacity;
        }
        updateGUITexts();
    }

    public void AddWorkers(int amount)
    {
        workerCount += amount;
        updateGUITexts();
    }

    public void RemoveWorkers(int amount)
    {
        workerCount -= amount;
        updateGUITexts();
    }

    public void IncreaseWoodCapacity(int addedCapacity)
    {
        woodCapacity += addedCapacity;
    }

    public void IncreaseFoodCapacity(int addedCapacity)
    {
        foodCapacity += addedCapacity;
    }

    public void DecreaseWoodCapacity(int removedCapacity)
    {
        woodCapacity -= removedCapacity;
        if (woodCapacity < 0)
        {
            woodCapacity = 0;
        }
        if (woodCount > woodCapacity)
        {
            woodCount = woodCapacity;
            updateGUITexts();
        }
    }

    public void DecreaseFoodCapacity(int removedCapacity)
    {
        foodCapacity -= removedCapacity;
        if (foodCapacity < 0)
        {
            foodCapacity = 0;
        }
        if (foodCount > foodCapacity)
        {
            foodCount = foodCapacity;
            updateGUITexts();
        }
    }

    private void updateGUITexts()
    {
        woodCountDisplay.text = "Wood " + woodCount.ToString();
        foodCountDisplay.text = "Food " + foodCount.ToString();
        workerCountDisplay.text = "Workers " + workerCount.ToString();
    }
}