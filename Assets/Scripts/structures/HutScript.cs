using UnityEngine;
using System.Collections;

public class HutScript : BaseStructure {
    private int workersAccomodated = 2;

    protected override void Start()
    {
        base.Start();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        ResourceManager.Instance.AddWorkers(workersAccomodated);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        ResourceManager.Instance.RemoveWorkers(workersAccomodated);
        Destroy(this.gameObject);
    }
}
