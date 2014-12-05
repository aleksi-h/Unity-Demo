using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float additionalBorderPadding; //to restrict camera from moving too close to borders
    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float maxFOV;
    public GUIText camPosDisplay;

    private Transform myTransform;
    private float cameraPointDistance; //ground distance between camera transform and the point in ground at which the camera is looking
    private GameObject cameraPointSimulator;
    private Transform cameraPoint;

    void Awake() {
        cameraPointSimulator = new GameObject("campoint");
        camPivotPoint = new GameObject("camtarget");
        cameraPoint = cameraPointSimulator.transform;
        myTransform = transform;
        cameraPoint.parent = myTransform;

        //calculate cameraPointDistance: A = B*sin(a) / sin(b) (a & b = angles, A & B = sides)
        float cameraAngle = 90 - myTransform.eulerAngles.x;
        float cameraHeight = myTransform.position.y;
        cameraPointDistance = (cameraHeight * Mathf.Sin(Mathf.Deg2Rad * cameraAngle)) / (Mathf.Sin(Mathf.Deg2Rad * (180 - (90 + cameraAngle))));
        cameraPointDistance += additionalBorderPadding;
    }

    void Start() {
        InputManager.OnDrag += Pan;
        InputManager.OnRotate += Rotate;
        InputManager.OnPinch += Zoom;
    }

    void Update() {
        //camPosDisplay.text = "Cam Pos: " + myTransform.position + " Cam rot: " + myTransform.eulerAngles;
    }

    private void Pan(Vector2 deltaPos) {
        float moveDistanceX = deltaPos.x * -scrollSpeed;
        float moveDistanceZ = deltaPos.y * -scrollSpeed;
        //cameraTransform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);

        float originalX = myTransform.eulerAngles.x;
        float originalZ = myTransform.eulerAngles.z;
        myTransform.Rotate(new Vector3(-originalX, 0, -originalZ)); //remove X and Z rotation from the camera to retain it's height when moved
        myTransform.Translate(moveDistanceX, 0, moveDistanceZ); //move the camera
        myTransform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations
        ApplyCorrections();
    }

    private void Zoom(float amount) {
        camera.fieldOfView += amount * zoomSpeed;
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minFOV, maxFOV);
    }

    private LayerMask groundLayerMask = 1 << 11;
    GameObject camPivotPoint;
    private void Rotate(float amount) {
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, 1000, groundLayerMask)) {
            Vector3 hitPos = hit.point;
            camPivotPoint.transform.position = hitPos;
            myTransform.parent = camPivotPoint.transform;
            camPivotPoint.transform.Rotate(0, amount * rotateSpeed, 0);
            myTransform.parent = null;
        }
        ApplyCorrections();
    }

    private Vector3 correctedPosition;
    //restrict movement inside playable area
    private void ApplyCorrections() {
        //correctedPosition = myTransform.position;
        //if (correctedPosition.x < Constants.PLAYABLE_AREA_MIN_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MIN_X; }
        //if (correctedPosition.x > Constants.PLAYABLE_AREA_MAX_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MAX_X; }
        //if (correctedPosition.z < Constants.PLAYABLE_AREA_MIN_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MIN_Z; }
        //if (correctedPosition.z > Constants.PLAYABLE_AREA_MAX_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MAX_Z; }
        //myTransform.position = correctedPosition;

        //simulate the position at which the camera is pointing, by doing Translate on the ghost transform "cameraPoint"
        cameraPoint.position = myTransform.position;
        cameraPoint.rotation = myTransform.rotation;
        cameraPoint.Translate(0, 0, cameraPointDistance);
        correctedPosition = myTransform.position;
        if (cameraPoint.position.x < Constants.PLAYABLE_AREA_MIN_X) { correctedPosition.x += Constants.PLAYABLE_AREA_MIN_X - cameraPoint.position.x; }
        if (cameraPoint.position.x > Constants.PLAYABLE_AREA_MAX_X) { correctedPosition.x += Constants.PLAYABLE_AREA_MAX_X - cameraPoint.position.x; }
        if (cameraPoint.position.z < Constants.PLAYABLE_AREA_MIN_Z) { correctedPosition.z += Constants.PLAYABLE_AREA_MIN_Z - cameraPoint.position.z; }
        if (cameraPoint.position.z > Constants.PLAYABLE_AREA_MAX_Z) { correctedPosition.z += Constants.PLAYABLE_AREA_MAX_Z - cameraPoint.position.z; }
        myTransform.position = correctedPosition;
    }
}
