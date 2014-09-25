using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure
{
    private int capacity;

    protected override void Start()
    {
        base.Start();
        capacity = 500;
        level = 1;
        maxHealth = 1000;
        health = maxHealth;

        ResourceManager.Instance.IncreaseCapacity(capacity);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        ResourceManager.Instance.DecreaseCapacity(capacity);
        Destroy(this.gameObject);
    }
}
