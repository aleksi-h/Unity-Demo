using UnityEngine;
using System.Collections;

public abstract class ProducerStructure : BaseStructure {
    protected float productionInterval; //how often
    protected int productionRate; //how much

    protected override void Start()
    {
       base.Start();
    }

    protected virtual void ProduceResources()
    {

    }
}
