using UnityEngine;
using System.Collections;

public class FollowGameObjectNGUI : MonoBehaviour {
    public Camera nguiCamera;
    public UIRoot uiRoot;
    private Transform myTransform;
    private GameObject target;
    private Transform targetTransform;
    private Vector3 offset;

    public void Awake() {
        myTransform = transform;
        offset = new Vector3(0, 0, 0);
    }

    public void LateUpdate() {
        if (target != null) {
            Vector3 pos = targetTransform.position + offset;
            pos = Camera.main.WorldToViewportPoint(pos);
            pos = nguiCamera.ViewportToWorldPoint(pos);
            pos = new Vector3((pos.x * uiRoot.manualHeight) / 2, (pos.y * uiRoot.manualHeight) / 2, 0);
            myTransform.localPosition = new Vector3(pos.x, pos.y, 0);
        }
    }

    public void setOffset(Vector3 offset) {
        this.offset = offset;
    }

    public void SetTarget(GameObject obj) {
        target = obj;
        if (target != null) {
            targetTransform = target.transform;
        }
    }
}
