using UnityEngine;
using System.Collections;

public class SawmillScript : BaseStructure {
    private int productionRate;

	protected override void Start () {
        base.Start();
        productionRate = 10;
        level = 1;
        maxHealth = 1000;
        health = maxHealth;

        InvokeRepeating("ProduceWood", 0, 1.0F);
	}

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        Destroy(this.gameObject);
    }

    private void ProduceWood()
    {
        ResourceManager.Instance.AddWood(productionRate);
    }
}
