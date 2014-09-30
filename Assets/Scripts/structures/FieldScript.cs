using UnityEngine;
using System.Collections;

public class FieldScript : ProducerStructure {

    protected override void Start()
    {
        base.Start();
        productionInterval = 1.0f;
        productionRate = 10;
        level = 1;
        maxHealth = 1000;
        health = maxHealth;

        InvokeRepeating("ProduceResources", 0, productionInterval);
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
        ResourceManager.Instance.AddFood(productionRate);
    }
}
