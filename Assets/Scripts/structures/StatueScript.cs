using UnityEngine;
using System.Collections;

public class StatueScript : UpgradableStructure, IProducer {

    #region IProducer
    [SerializeField]
    private float productionInterval;
    public float ProductionInterval {
        get { return productionInterval; }
    }

    [SerializeField]
    private Resource baseProductionRate;
    public Resource BaseProductionRate {
        get { return baseProductionRate; }
    }

    public void ProduceResources() {
        ResourceManager.Instance.AddResources(baseProductionRate);
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        maxHealth = 1000;
        health = maxHealth;
    }

    public override void Upgrade() {
        base.Upgrade();
        CancelInvoke("ProduceResources");
    }

    protected override void FinishUpgrade(GameObject upgraded) {
        Destroy(gameObject);
    }

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {

    }
}
