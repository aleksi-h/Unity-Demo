using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IRemovable, IDamageable {
    public Resource cost;
    public int buildTime;
    
    protected bool structureActive;
    protected int maxHealth;
    protected int level;
    protected int health;
    protected StructureType type;
    public StructureType Type
    {
        get { return type; }
    }

    #region IRemovable
    public abstract void Remove();
    public abstract bool RemovalAllowed();
    #endregion

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    protected virtual void Awake()
    {
    }

	protected virtual void Start () 
    {
	}
	
	protected virtual void Update ()
    {
	}

    public virtual void Activate(){
        structureActive = true;
    }
}
