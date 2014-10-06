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
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        ResourceManager.Instance.IncreaseResourceCapacity(capacity);
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
