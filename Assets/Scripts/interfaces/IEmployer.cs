using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IEmployer {
    int MinWorkerCount {
        get;
    }
    int MaxWorkerCount {
        get;
    }
    List<GameObject> Workers {
        get;
    }
    Resource ProductionBoostPerWorker {
        get;
    }
    Resource ProductionBoost {
        get;
    }

    void AddWorker();
    void FreeWorker();
    void LoadWorkers(int count);
    void SetWorkers(List<GameObject> workers);
}