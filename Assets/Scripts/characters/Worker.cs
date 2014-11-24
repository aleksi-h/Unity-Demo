using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour {
    public static int EFFICIENCY = 100;
    public int speed;

    private Transform myTransform;
    private NavMeshAgent agent;
    private GameObject destination;
    private bool hasNewDestination;

    public void AssignToStructure(GameObject structure) {
        destination = structure;
        hasNewDestination = true;
    }

    public void Free() {
        destination = null;
        hasNewDestination = true;
        
    }

    public void Awake() {
        myTransform = transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void Update() {
        if (hasNewDestination) {
            if (destination == null) { agent.SetDestination(new Vector3(10, 0, 0)); }
            else { agent.SetDestination(destination.transform.position); }
        }
    }
}
