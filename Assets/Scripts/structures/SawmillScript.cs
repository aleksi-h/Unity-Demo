using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SawmillScript : BaseStructure, IUpgradeable, IProducer, IEmployer, IRemovable {

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
        Remove(); //loppuuko scriptin suoritus?
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
