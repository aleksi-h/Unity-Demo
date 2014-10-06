﻿using UnityEngine;
using System.Collections;

public class HutScript : BaseStructure {
    private int workersAccomodated = 2;

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.hut;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Activate()
    {
        ResourceManager.Instance.AddWorkers(workersAccomodated);
    }

    public override void Remove()
    {
        ResourceManager.Instance.RemoveWorkers(workersAccomodated);
        Destroy(this.gameObject);
    }

    public override void Damage(int amount)
    {
    }
}