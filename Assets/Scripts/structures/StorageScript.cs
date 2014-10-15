﻿using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure, IRemovable
{
    public Resource capacity;

    #region IRemovable
    public void Remove() {
        ResourceManager.Instance.DecreaseResourceCapacity(capacity);
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
        type = StructureType.Storage;
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
        ResourceManager.Instance.IncreaseResourceCapacity(capacity);
    }

    public override void Damage(int amount)
    {
    }
}
