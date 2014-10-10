using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager> {
    
    public Camera mainCamera;
    public float speed;
    public float additionalBorderPadding; //to restrict camera from moving too close to borders

    public GUIText camPosDisplay;

    private Transform cameraTransform;
    private float cameraPointDistance; //ground distance between camera transform and the point in ground at which the camera is pointing
    private GameObject cameraPointSimulator;
    private Transform cameraPoint;
    Vector3 correctedPosition;

    //TODO use events to send touches?

    public override void Awake()
    {
        cameraPointSimulator = new GameObject();
        cameraPoint = cameraPointSimulator.transform;
        cameraTransform = mainCamera.transform;

        //calculate cameraPointDistance: A = B*sin(a) / sin(b) (a & b = angles, A & B = sides)
        float cameraAngle = 90 - cameraTransform.eulerAngles.x;
        float cameraHeight = cameraTransform.position.y;
        cameraPointDistance = (cameraHeight * Mathf.Sin(Mathf.Deg2Rad * cameraAngle)) / (Mathf.Sin(Mathf.Deg2Rad * (180 - (90 + cameraAngle))));
        cameraPointDistance += additionalBorderPadding;
    }

    void Update()
    {

        //**********************ROTATE**********************
        if (Input.touchCount == 2)
        {
            Touch finger1;
            Touch finger2;

            //finger1 is always on the left, to keep turn direction constant, regardless of which finger touched the screen first
            if (Input.GetTouch(0).position.x < Input.GetTouch(1).position.x)
            {
                finger1 = Input.GetTouch(0);
                finger2 = Input.GetTouch(1);
            }
            else
            {
                finger1 = Input.GetTouch(1);
                finger2 = Input.GetTouch(0);
            }

            if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
            {
                float finger1DistanceMoved = finger1.deltaPosition.y;
                float finger2DistanceMoved = -finger2.deltaPosition.y;
                float combinedDistanceMoved = finger1DistanceMoved + finger2DistanceMoved;

                float originalX = cameraTransform.eulerAngles.x;
                float originalZ = cameraTransform.eulerAngles.z;
                cameraTransform.Rotate(new Vector3(-originalX, 0, -originalZ)); //set X and Z to 0 to isolate rotation to Y-axis
                cameraTransform.Rotate(new Vector3(0, combinedDistanceMoved, 0)); //apply rotation to Y-axis
                cameraTransform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations

                ApplyCorrections();
            }
        }

        //**********************PAN**********************
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float moveDistanceX = Input.GetTouch(0).deltaPosition.x * -speed;
            float moveDistanceZ = Input.GetTouch(0).deltaPosition.y * -speed;
            //cameraTransform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);

            float originalX = cameraTransform.eulerAngles.x;
            float originalZ = cameraTransform.eulerAngles.z;
            cameraTransform.Rotate(new Vector3(-originalX, 0, -originalZ)); //remove X and Z rotation from the camera to retain it's height when moved
            cameraTransform.Translate(moveDistanceX, 0, moveDistanceZ); //move the camera
            cameraTransform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations

            ApplyCorrections();
        }

        //**********************TAP**********************
        else if (Input.touchCount == 1)
        {
             BuildingManager.Instance.OnClick();
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0))
        {
            BuildingManager.Instance.OnClick();
        }
#endif

        camPosDisplay.text = "Cam Pos: " + cameraTransform.position + " Cam rot: "+cameraTransform.eulerAngles;
    }


    //apply corrections to camera position to restrict movement inside playable area
    private void ApplyCorrections()
    {
        correctedPosition = cameraTransform.position;
        if (correctedPosition.x < Constants.PLAYABLE_AREA_MIN_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MIN_X; }
        if (correctedPosition.x > Constants.PLAYABLE_AREA_MAX_X) { correctedPosition.x = Constants.PLAYABLE_AREA_MAX_X; }
        if (correctedPosition.z < Constants.PLAYABLE_AREA_MIN_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MIN_Z; }
        if (correctedPosition.z > Constants.PLAYABLE_AREA_MAX_Z) { correctedPosition.z = Constants.PLAYABLE_AREA_MAX_Z; }
        cameraTransform.position = correctedPosition;

        //simulate the position at which the camera is pointing, by doing Translate on the ghost transform "cameraPoint"
        cameraPoint.position = cameraTransform.position;
        cameraPoint.rotation = cameraTransform.rotation;
        cameraPoint.Translate(0, 0, cameraPointDistance);
        correctedPosition = cameraTransform.position;
        if (cameraPoint.position.x < Constants.PLAYABLE_AREA_MIN_X) { correctedPosition.x += Constants.PLAYABLE_AREA_MIN_X - cameraPoint.position.x; }
        if (cameraPoint.position.x > Constants.PLAYABLE_AREA_MAX_X) { correctedPosition.x += Constants.PLAYABLE_AREA_MAX_X - cameraPoint.position.x; }
        if (cameraPoint.position.z < Constants.PLAYABLE_AREA_MIN_Z) { correctedPosition.z += Constants.PLAYABLE_AREA_MIN_Z - cameraPoint.position.z; }
        if (cameraPoint.position.z > Constants.PLAYABLE_AREA_MAX_Z) { correctedPosition.z += Constants.PLAYABLE_AREA_MAX_Z - cameraPoint.position.z; }
        cameraTransform.position = correctedPosition;
    }

#if UNITY_EDITOR
#endif
}

