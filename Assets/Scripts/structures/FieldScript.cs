using UnityEngine;
using System.Collections;

public class FieldScript : ProducerStructure {

    protected override void Start()
    {
        base.Start();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        Destroy(this.gameObject);
    }

    public override void Upgrade()
    {
    }

    public override void Damage(int amount)
    {
    }
    protected override void ProduceResources()
    {
        ResourceManager.Instance.AddFood(productionRate);
    }
}
