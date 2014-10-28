using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UpgradableStructure : BaseStructure, IUpgradeable {

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
        StartLongProcess(UpgradeProcess, duration);
    }
    #endregion

    protected virtual void UpgradeProcess() {
        GUIManager.Instance.RemoveTimerDisplay(timerDisplay);

        //"upgraded".Start() will be called 1 frame after this, so things that need to be done before that, like releasing workers, can be done in FinishUpgrade()
        GameObject upgraded = (GameObject)Instantiate(nextLevelPrefab, myTransform.position, Quaternion.identity);
        GUIManager.Instance.UpgradeFinished(gameObject, upgraded);
        upgraded.GetComponent<BaseStructure>().Activate();
        if (onTapRegistered) {
            upgraded.GetComponent<BaseStructure>().Move();
        }
        FinishUpgrade();
    }

    //building specific tasks. mainly for correctly removing self
    protected abstract void FinishUpgrade();
}