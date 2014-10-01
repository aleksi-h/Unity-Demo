using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IRemovable, IUpgradeable, IDamageable {
    public int costInWood;
    public int costInFood;

    protected StructureType type;
    protected int maxHealth;
    protected int level;
    protected int health;

    public StructureType Type
    {
        get { return type; }
    }

    protected virtual void Awake()
    {
    }

	protected virtual void Start () 
    {
	}
	
	protected virtual void Update ()
    {
	}

    public abstract void Remove();

    public abstract void Upgrade();

    public abstract void Damage(int amount);
}
