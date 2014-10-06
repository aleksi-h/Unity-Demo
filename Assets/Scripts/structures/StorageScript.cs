using UnityEngine;
using System.Collections;

public class StorageScript : BaseStructure
{
    public Resource capacity;

    protected override void Awake()
    {
        base.Awake();
        level = 1;
        maxHealth = 1000;
        health = maxHealth;
        type = StructureType.storage;
        ResourceManager.Instance.IncreaseResourceCapacity(capacity);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Remove()
    {
        ResourceManager.Instance.DecreaseResourceCapacity(capacity);
        Destroy(this.gameObject);
    }

    public override void Damage(int amount)
    {
    }
}
