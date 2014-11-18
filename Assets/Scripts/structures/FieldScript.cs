using UnityEngine;
using System.Collections;

public class FieldScript : UpgradableStructure, IProducer, IRemovable {

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

    #region IRemovable
    public void Remove() {
        gridComponent.DetachFromGrid();
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive&&gridComponent.CanBeRemoved();
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

    protected override void FinishUpgrade() {
        Remove();
    }

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {
    }
}
