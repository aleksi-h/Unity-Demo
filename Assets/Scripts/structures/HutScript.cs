using UnityEngine;
using System.Collections;

public class HutScript : BaseStructure {
    [SerializeField]
    private int workerCapacity = 2;

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
    }

    protected override void Update() {
        base.Update();
    }

    public override void Activate() {
        base.Activate();
        ResourceManager.Instance.AddWorkers(workerCapacity);
    }

    public override void Damage(int amount) {
    }
}
