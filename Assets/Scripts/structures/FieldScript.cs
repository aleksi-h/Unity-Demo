﻿using UnityEngine;
using System.Collections;

public class FieldScript : BaseStructure, IUpgradeable, IProducer
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

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.field;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Activate()
    {
        InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    public override void Remove()
    {
        Destroy(this.gameObject);
    }

    public override void Damage(int amount)
    {
    }
}
