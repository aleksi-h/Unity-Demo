using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SawmillScript : BaseStructure, IUpgradeable, IProducer, IEmployer, IRemovable {

    #region IUpgradeable
    public GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab {

        get { return nextLevelPrefab; }
    }

    public void PrepareForUpgrade() {
        CancelInvoke("ProduceResources");
    }

    public bool UpgradeAllowed() {
        return structureActive;
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

    // TODO move grid reference here
    #region IRemovable
    public void Remove() {
        while (workers.Count > 0) {
            ResourceManager.Instance.ReturnWorker(workers[0]);
            workers.RemoveAt(0);
        }
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive;
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

    private List<GameObject> workers;
    public List<GameObject> Workers {
        get { return workers; }
    }

    public void AddWorker() {
        workers.Add(ResourceManager.Instance.RequestWorker(gameObject));
    }

    public void FreeWorker() {
        ResourceManager.Instance.ReturnWorker(workers[0]);
        workers.RemoveAt(0);
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.Sawmill;
        workers = new List<GameObject>(minWorkerCount);
    }

    protected override void Start() {
        base.Start();
        for (int i = 0; i < minWorkerCount; i++) {
            AddWorker();
        }
    } 

    public override void Activate() {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount) {
    }
}
