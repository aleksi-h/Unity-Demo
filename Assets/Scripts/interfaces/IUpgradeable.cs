using UnityEngine;
using System.Collections;

public interface IUpgradeable {

    GameObject NextLevelPrefab { get; }
    void Upgrade();
    void ContinueUpgrade(float timeLeft);
    bool UpgradeAllowed();
}