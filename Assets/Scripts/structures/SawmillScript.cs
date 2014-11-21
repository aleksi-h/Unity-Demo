using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SawmillScript : UpgradableStructure, IProducer, IEmployer, IRemovable {

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
        ResourceManager.Instance.AddResources(baseProductionRate + productionBoost);
    }
    #endregion

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
    [SerializeField]
    private int minWorkerCount;
    public int MinWorkerCount {
        get { return minWorkerCount; }
    }

    [SerializeField]
    private int maxWorkerCount;
    public int MaxWorkerCount {
        get { return maxWorkerCount; }
    }

    [SerializeField]
    private Resource productionBoostPerWorker;
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

    public void LoadWorkers(int count) {
        for (int i = 0; i < count; i++) {
            workers.Add(ResourceManager.Instance.CreateWorker(gameObject));
        }
        int boost = workers.Count - minWorkerCount;
        if (boost >= 0) {
            productionBoost = new Resource(boost * productionBoostPerWorker.wood,
                boost * productionBoostPerWorker.food, boost * productionBoostPerWorker.currency);
        }
    }

    public void AddWorker() {
        workers.Add(ResourceManager.Instance.RequestWorker(gameObject));
        int boost = workers.Count - minWorkerCount;
        if (boost >= 0) {
            productionBoost = new Resource(boost * productionBoostPerWorker.wood,
                boost * productionBoostPerWorker.food, boost * productionBoostPerWorker.currency);
        }
    }

    public void SetWorkers(List<GameObject> workers) {
        this.workers = workers;
        foreach (GameObject worker in workers) {
            worker.GetComponent<Worker>().AssignToStructure(gameObject);
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
        maxHealth = 1000;
        health = maxHealth;
        workers = new List<GameObject>(minWorkerCount);
        productionBoost = new Resource(0, 0, 0);
    }

    protected override void Start() {
        base.Start();
    }

    public override void Upgrade() {
        base.Upgrade();
        CancelInvoke("ProduceResources");
    }

    public override void Build() {
        base.Build();
        for (int i = 0; i < minWorkerCount; i++) {
            AddWorker();
        }
    }

    protected override void FinishUpgrade(GameObject upgraded) {
        upgraded.GetInterface<IEmployer>().SetWorkers(workers);
        Destroy(this.gameObject);
    }

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {
    }
}
