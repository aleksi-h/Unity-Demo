using UnityEngine;
using System.Collections;

public class FieldScript : ProducerStructure {

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.field;
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
