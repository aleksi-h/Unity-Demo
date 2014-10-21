using UnityEngine;
using System.Collections;

public class FieldScript : BaseStructure, IUpgradeable, IProducer, IRemovable {

    #region IUpgradeable
    public GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab {

        get { return nextLevelPrefab; }
    }

    public bool UpgradeAllowed() {
        return structureActive;
    }

    public void Upgrade() {
        structureActive = false;
        CancelInvoke("ProduceResources"); 
        gameObject.GetComponent<BoxCollider>().enabled = false;
        float duration = nextLevelPrefab.GetComponent<BaseStructure>().buildTime;
        StartLongProcess(UpgradeProcess, duration);
    }

    private void UpgradeProcess() {
        GUIManager.Instance.RemoveTimerDisplay(timerDisplay);
        GameObject upgraded = (GameObject)Instantiate(nextLevelPrefab, myTransform.position, Quaternion.identity);
        Remove();
        upgraded.GetComponent<BaseStructure>().Activate();
    }
    #endregion

    #region IProducer
    public float productionInterval;
    public float ProductionInterval {
        get { return productionInterval; }
    }

    public Resource producedPerInterval;
    public Resource ProducedPerInterval {
        get { return producedPerInterval; }
    }

    public void ProduceResources() {
        ResourceManager.Instance.AddResources(producedPerInterval);
    }
    #endregion

    #region IRemovable
    public void Remove() {
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive;
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.Field;
    }

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {
    }
}
