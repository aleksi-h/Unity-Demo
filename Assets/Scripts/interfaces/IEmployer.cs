﻿using UnityEngine;
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

    void AddWorker();
    void FreeWorker();
}