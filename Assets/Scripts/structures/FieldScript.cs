using UnityEngine;
using System.Collections;

public class FieldScript : ProducerStructure, IUpgradeable 
{

    #region IUpgradeable
    public Resource upgradeCost;
    public Resource UpgradeCost
    {
        get { return upgradeCost; }
    }

    public int upgradeDuration;
    public int UpgradeDuration
    {
        get { return upgradeDuration; }
    }

    public GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab
    {
        get { return nextLevelPrefab; }
    }

    public void Upgrade()
    {
        CancelInvoke("ProduceResources");
    }
    #endregion

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

    public override void Damage(int amount)
    {
    }

    protected override void ProduceResources()
    {
        base.ProduceResources();
        ResourceManager.Instance.AddResources(producedPerInterval);
    }
}
