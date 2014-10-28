using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float scrollSpeed;
    public float zoomSpeed;
    public float rotateSpeed;
    public float additionalBorderPadding; //to restrict camera from moving too close to borders
    public float minFOV;
    public float maxFOV;
    public GUIText camPosDisplay;

    private Transform myTransform;
    private float cameraPointDistance; //ground distance between camera transform and the point in ground at which the camera is pointing
    private GameObject cameraPointSimulator;
    private Transform cameraPoint;
    Vector3 correctedPosition;

    void Awake() {
        cameraPointSimulator = new GameObject();
        cameraPoint = cameraPointSimulator.transform;
        myTransform = transform;

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
        camPosDisplay.text = "Cam Pos: " + myTransform.position + " Cam rot: " + myTransform.eulerAngles;
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

    private void Rotate(float amount) {
        float originalX = myTransform.eulerAngles.x;
        float originalZ = myTransform.eulerAngles.z;
        myTransform.Rotate(new Vector3(-originalX, 0, -originalZ)); //set X and Z to 0 to isolate rotation to Y-axis
        myTransform.Rotate(new Vector3(0, amount * rotateSpeed, 0)); //apply rotation to Y-axis
        myTransform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations
        ApplyCorrections();
    }

    //apply corrections to camera position to restrict movement inside playable area
    private void ApplyCorrections() {
        correctedPosition = myTransform.position;
        if (correctedPosition.x < Constants.PLAYABLE_AREA_MIN_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MIN_X; }
        if (correctedPosition.x > Constants.PLAYABLE_AREA_MAX_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MAX_X; }
        if (correctedPosition.z < Constants.PLAYABLE_AREA_MIN_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MIN_Z; }
        if (correctedPosition.z > Constants.PLAYABLE_AREA_MAX_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MAX_Z; }
        myTransform.position = correctedPosition;

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
