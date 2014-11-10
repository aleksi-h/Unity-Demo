using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SawmillScript : UpgradableStructure, IProducer, IEmployer, IRemovable {

    #region IProducer
    public float productionInterval;
    public float ProductionInterval {
        get { return productionInterval; }
    }

    public Resource baseProductionRate;
    public Resource BaseProductionRate {
        get { return baseProductionRate; }
    }

    public void ProduceResources() {
        ResourceManager.Instance.AddResources(baseProductionRate + productionBoost);
    }
    #endregion

    // TODO move grid reference here
    #region IRemovable
    public void Remove() {
        while (workers.Count > 0) {
            ResourceManager.Instance.ReleaseWorker(workers[0]);
            workers.RemoveAt(0);
        }
        gridComponent.DetachFromGrid();
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive && gridComponent.CanBeRemoved();
    }
    #endregion

    #region IEmployer
    public int minWorkerCount;
    public int MinWorkerCount {
        get { return minWorkerCount; }
    }

    public int maxWorkerCount;
    public int MaxWorkerCount {
        get { return maxWorkerCount; }
    }

    public Resource productionBoostPerWorker;
    public Resource ProductionBoostPerWorker {
        get { return productionBoostPerWorker; }
    }

    private Resource productionBoost;
    public Resource ProductionBoost {
        get { return productionBoost; }
    }

    private List<GameObject> workers;
    public List<GameObject> Workers {
        get { return workers; }
    }

    public void AddWorker() {
        workers.Add(ResourceManager.Instance.RequestWorker(gameObject));
        int boost = workers.Count - minWorkerCount;
        if (boost >= 0) {
            productionBoost = new Resource(boost * productionBoostPerWorker.wood, 
                boost * productionBoostPerWorker.food, boost * productionBoostPerWorker.currency);
        }
    }

    public void FreeWorker() {
        ResourceManager.Instance.ReleaseWorker(workers[0]);
        workers.RemoveAt(0);
        int boost = workers.Count - minWorkerCount;
        if (boost >= 0) {
            productionBoost = new Resource(boost * productionBoostPerWorker.wood,
                boost * productionBoostPerWorker.food, boost * productionBoostPerWorker.currency);
        }
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        workers = new List<GameObject>(minWorkerCount);
    }

    protected override void Start() {
        base.Start();
        for (int i = 0; i < minWorkerCount; i++) {
            AddWorker();
        }
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
