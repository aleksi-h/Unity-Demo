using UnityEngine;
using System.Collections;

public class HutScript : BaseStructure, IRemovable {
    private int workersAccomodated = 2;

    #region IRemovable
    public void Remove() {
        ResourceManager.Instance.RemoveWorkers(workersAccomodated);
        Destroy(this.gameObject);
    }

    public bool RemovalAllowed() {
        return structureActive;
    }
    #endregion

    protected override void Awake() {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.Hut;
    }

    protected override void Update() {
        base.Update();
    }

    public override void Activate() {
        base.Activate();
        ResourceManager.Instance.AddWorkers(workersAccomodated);
    }

    public override void Damage(int amount) {
    }
}
