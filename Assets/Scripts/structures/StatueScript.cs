using UnityEngine;
using System.Collections;

public class StatueScript : UpgradableStructure, IProducer {

    #region IProducer
    public float productionInterval;
    public float ProductionInterval {
        get { return productionInterval; }
    }

    public Resource producedPerInterval;
    public Resource BaseProductionRate {
        get { return producedPerInterval; }
    }

    public void ProduceResources() {
        ResourceManager.Instance.AddResources(producedPerInterval);
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.Special;
    }

    public override void Upgrade() {
        base.Upgrade();
        CancelInvoke("ProduceResources");
    }

    protected override void FinishUpgrade() {
        Destroy(gameObject);
    }

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {

    }
}
