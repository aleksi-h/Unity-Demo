using UnityEngine;
using System.Collections;

public interface IUpgradeable
{

    GameObject NextLevelPrefab
    {
        get;
    }

    void PrepareForUpgrade();
    bool UpgradeAllowed();
}