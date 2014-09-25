using UnityEngine;
using System.Collections;

public class TowerScript : BaseStructure {

    protected override void Start()
    {
        base.Start();
	}

    protected override void Update()
    {
        base.Update();
	}

    public override void Remove()
    {
        Destroy(this.gameObject);
    }
}
