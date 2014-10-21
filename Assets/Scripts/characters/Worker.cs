using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour {
    public static int EFFICIENCY = 100;
    public int speed;

    private Transform myTransform;
    private GameObject location;
    private bool hasNewDestination;

    public void AssignToStructure(GameObject structure) {
        location = structure;
        hasNewDestination = true;
    }

    public void Free() {
        location = null;
        hasNewDestination = true;
    }

    public void Awake() {
        myTransform = transform;
    }

    public void Update() {
        if (myTransform.position.Equals(location)) {
            hasNewDestination = false;
        }
        if (hasNewDestination) {
            if (location == null) {
                myTransform.position = Vector3.MoveTowards(myTransform.position, new Vector3(10, 0, 0), speed * Time.deltaTime);
            }
            else {
                myTransform.position = Vector3.MoveTowards(myTransform.position, location.transform.position, speed * Time.deltaTime);
            }
        }
    }
}
