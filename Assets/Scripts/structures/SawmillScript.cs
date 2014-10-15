﻿using UnityEngine;
using System.Collections;

public class SawmillScript : BaseStructure, IUpgradeable, IProducer
{

    #region IUpgradeable
    public GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab
    {

        get { return nextLevelPrefab; }
    }

    public void PrepareForUpgrade()
    {
        CancelInvoke("ProduceResources");
    }

    public bool UpgradeAllowed()
    {
        return structureActive;
    }
    #endregion

    #region IProducer
    public float productionInterval;
    public float ProductionInterval
    {
        get { return productionInterval; }
    }

    public Resource producedPerInterval;
    public Resource ProducedPerInterval
    {
        get { return producedPerInterval; }
    }

    public void ProduceResources()
    {
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

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.Sawmill;
    }

    public override void Activate()
    {
        base.Activate();
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Damage(int amount)
    {
    }
}
