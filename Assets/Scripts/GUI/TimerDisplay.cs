using UnityEngine;
using System.Collections;

public class TimerDisplay : MonoBehaviour {
    private Transform myTransform;
    private GameObject target;
    private Transform targetTransform;

    public void Awake() {
        myTransform = transform;
    }

    public void Update() {
        if (target != null) {
            myTransform.position = Camera.main.WorldToViewportPoint(targetTransform.position);
        }
    }

    public void FollowObject(GameObject obj) {
        target = obj;
        targetTransform = target.transform;
    }
}
