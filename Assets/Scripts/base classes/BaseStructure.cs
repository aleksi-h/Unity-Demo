using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IRemovable {
    public int maxHealth;

    protected int level;
    protected int health;
    


	protected virtual void Start () {
	
	}
	
	protected virtual void Update () {
	
	}

    public abstract void Remove();
}
