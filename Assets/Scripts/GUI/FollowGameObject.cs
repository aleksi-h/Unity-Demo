using UnityEngine;
using System.Collections;

public class FollowGameObject : MonoBehaviour {
    private Transform myTransform;
    private GameObject target;
    private Transform targetTransform;

    public void Awake() {
        myTransform = transform;
    }

    public void LateUpdate() {
        if (target != null) {
            myTransform.position = Camera.main.WorldToViewportPoint(targetTransform.position);
        }
    }

    public void FollowObject(GameObject obj) {
        target = obj;
        targetTransform = target.transform;
    }
}
