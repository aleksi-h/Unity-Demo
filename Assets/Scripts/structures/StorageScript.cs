using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure
{
    private int woodCapacity;
    private int foodCapacity;

    protected override void Start()
    {
        base.Start();
        woodCapacity = 500;
        foodCapacity = 500;
        level = 1;
        maxHealth = 1000;
        health = maxHealth;

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
