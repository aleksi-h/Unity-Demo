using UnityEngine;
using System.Collections;

public interface IProducer {
    float ProductionInterval {
        get;
    }

    Resource BaseProductionRate {
        get;
    }

    void ProduceResources();
}
