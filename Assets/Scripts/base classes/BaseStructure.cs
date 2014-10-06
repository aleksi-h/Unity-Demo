using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IRemovable, IDamageable {
    public Resource cost;
    
    protected int maxHealth;
    protected int level;
    protected int health;
    protected StructureType type;
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

    public abstract void Damage(int amount);
}
