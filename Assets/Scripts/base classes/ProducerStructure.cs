using UnityEngine;
using System.Collections;

public abstract class ProducerStructure : BaseStructure {
    public float productionInterval; //how often
    public Resource producedPerInterval; //how much

    protected override void Start()
    {
       base.Start();
       InvokeRepeating("ProduceResources", 0, productionInterval);
    }

    protected virtual void ProduceResources()
    {

    }
}
