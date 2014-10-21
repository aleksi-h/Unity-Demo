using UnityEngine;
using System.Collections;

public interface IProducer {
    float ProductionInterval {
        get;
    }

    Resource ProducedPerInterval {
        get;
    }

    void ProduceResources();
}
