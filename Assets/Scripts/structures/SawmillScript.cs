using UnityEngine;
using System.Collections;

public class SawmillScript : ProducerStructure {

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.sawmill;
    }

	protected override void Start () {
        base.Start();
	}

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        Destroy(this.gameObject);
    }

    protected override void ProduceResources()
    {
        ResourceManager.Instance.AddWood(productionRate);
    }

    public override void Upgrade()
    {
    }

    public override void Damage(int amount)
    {
    }
}
