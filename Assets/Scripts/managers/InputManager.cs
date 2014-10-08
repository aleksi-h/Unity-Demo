using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager> {
    
    public Camera mainCamera;
    public float speed;

    //TODO use events to send touches?

    void Update()
    {

        //**********************rotating**********************
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

                float originalX = mainCamera.transform.eulerAngles.x;
                float originalZ = mainCamera.transform.eulerAngles.z;
                mainCamera.transform.Rotate(new Vector3(-originalX, 0, -originalZ)); //set X and Z to 0 to isolate movement to Y-axis
                mainCamera.transform.Rotate(new Vector3(0, combinedDistanceMoved, 0)); //apply rotation to Y-axis
                mainCamera.transform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations
            }
        }
        //**********************panning**********************
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            //mainCamera.transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);

            float originalX = mainCamera.transform.eulerAngles.x;
            float originalZ = mainCamera.transform.eulerAngles.z;
            mainCamera.transform.Rotate(new Vector3(-originalX, 0, -originalZ)); //remove X and Z rotation from the camera to retain it's height when moved
            mainCamera.transform.Translate(-touchDeltaPosition.x * speed, 0, -touchDeltaPosition.y * speed); //move the camera
            mainCamera.transform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations
            
        }
        //**********************tapping**********************
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
    }

#if UNITY_EDITOR
#endif
}

