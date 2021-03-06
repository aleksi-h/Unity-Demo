﻿using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure, IRemovable
{
    [SerializeField]
    private Resource capacity;

    #region IRemovable
    public void Remove() {
        ResourceManager.Instance.DecreaseResourceCapacity(capacity);
        gridComponent.DetachFromGrid();
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive && gridComponent.CanBeRemoved();
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        maxHealth = 1000;
        health = maxHealth;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        base.Activate();
        if (isNew) { ResourceManager.Instance.IncreaseResourceCapacity(capacity); }
    }

    public override void Damage(int amount)
    {
    }
}
