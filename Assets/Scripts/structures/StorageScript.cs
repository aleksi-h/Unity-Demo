using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure
{
    private int woodCapacity = 500;
    private int foodCapacity = 500;

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.storage;
        ResourceManager.Instance.IncreaseWoodCapacity(woodCapacity);
        ResourceManager.Instance.IncreaseFoodCapacity(foodCapacity);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        ResourceManager.Instance.DecreaseWoodCapacity(woodCapacity);
        ResourceManager.Instance.DecreaseFoodCapacity(foodCapacity);
        Destroy(this.gameObject);
    }

    public override void Upgrade()
    {
    }

    public override void Damage(int amount)
    {
    }
}
