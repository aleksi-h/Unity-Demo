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

    public override void Build(Grid grid) {
        base.Build(grid);
    }

    protected override void FinishUpgrade(GameObject upgraded) {
        gridComponent.grid.LevelUp(level + 1);
        Destroy(this.gameObject);
    }

    public override void Activate() {
        base.Activate();
    }

    public override void Damage(int amount) {
    }
}