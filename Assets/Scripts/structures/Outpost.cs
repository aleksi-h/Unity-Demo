using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outpost : UpgradableStructure {

    protected override void Awake() {
        base.Awake();
        maxHealth = 1000;
        health = maxHealth;
    }

    protected override void Start() {
        base.Start();
    }

    public override void Upgrade() {
        base.Upgrade();
    }

    public override void Build() {
        base.Build();
    }

    protected override void FinishUpgrade(GameObject upgraded) {
        Grid.Instance.LevelUp(level + 1);
        Destroy(this.gameObject);
    }

    public override void Activate() {
        base.Activate();
    }

    public override void Damage(int amount) {
    }
}