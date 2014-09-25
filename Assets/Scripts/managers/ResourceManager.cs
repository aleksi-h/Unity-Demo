using UnityEngine;
using System.Collections;

/*
 * A singleton Resource manager that persists between scenes 
 * 
 */
public class ResourceManager : Singleton<ResourceManager>
{    
    public GUIText woodCountDisplay;
    private long woodCount;
    private long woodCapacity;
   
    void Start()
    {
        woodCapacity = 0;
        woodCountDisplay.text = "Wood " + woodCount.ToString();
    }

    public void AddWood(int addedWood)
    {
        woodCount += addedWood;
        if (woodCount > woodCapacity)
        {
            woodCount = woodCapacity;
        }
        updateGUIText();
    }

    public void IncreaseCapacity(int addedCapacity)
    {
        woodCapacity += addedCapacity;
    }

    public void DecreaseCapacity(int removedCapacity)
    {
        woodCapacity -= removedCapacity;
        if (woodCapacity < 0)
        {
            woodCapacity = 0;
        }
        if (woodCount > woodCapacity)
        {
            woodCount = woodCapacity;
            updateGUIText();
        }
    }

    private void updateGUIText()
    {
        woodCountDisplay.text = "Wood " + woodCount.ToString();
    }
}