using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UpgradableStructure : BaseStructure, IUpgradeable {
    [HideInInspector]
    public float upgradeTimeLeft;
    [HideInInspector]
    public bool isUpgrading;

    #region IUpgradeable
    public GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab {
        get { return nextLevelPrefab; }
    }

    public bool UpgradeAllowed() {
        return structureActive;
    }

    public virtual void Upgrade() {
        structureActive = false;
        float duration = nextLevelPrefab.GetComponent<BaseStructure>().buildTime;
        StartCoroutine(StartDelayedOperation(UpgradeProcess, duration));
        isUpgrading = true;
    }

    public virtual void ContinueUpgrade(float timeLeft) {
        StartCoroutine(StartDelayedOperation(UpgradeProcess, timeLeft));
        isUpgrading = true;
    }
    #endregion

    protected virtual void UpgradeProcess() {
        Destroy(timerDisplay);

        //"upgraded".Start() will be called 1 frame after this, so things that need to be done before that, like releasing workers, can be done in FinishUpgrade()
        GameObject upgraded = (GameObject)Instantiate(nextLevelPrefab, myTransform.position, Quaternion.identity);
        upgraded.GetComponent<BaseStructure>().Activate();

        //replace all references to the gameobject before removal
        gridComponent.Replace(upgraded.GetComponent<GridComponent>());
        GUIManager.Instance.StructureUpgraded(gameObject, upgraded);

        isUpgrading = false;
        AudioManager.Instance.PlayOnce(AudioManager.Instance.buildingUpgraded);
        FinishUpgrade(upgraded);
    }

    //building specific tasks.
    protected abstract void FinishUpgrade(GameObject upgraded);
}