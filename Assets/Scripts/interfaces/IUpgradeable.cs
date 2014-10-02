using UnityEngine;
using System.Collections;

public interface IUpgradeable
{
    Resource UpgradeCost
    {
        get;
    }

    int UpgradeDuration
    {
        get;
    }

    GameObject NextLevelPrefab
    {
        get;
    }

    void Upgrade();
}