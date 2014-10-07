using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager> {
    
    public Camera camera;
    public float speed;

    void Update()
    {

        //**********************rotating**********************
        if (Input.touchCount == 2)
        {
            Touch finger1 = Input.GetTouch(0);
            Touch finger2 = Input.GetTouch(1);

            if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
            {
                float finger1DistanceMoved = finger1.deltaPosition.y;
                float finger2DistanceMoved = -finger2.deltaPosition.y;
                float combinedDistanceMoved = finger1DistanceMoved + finger2DistanceMoved;

                float originalX = camera.transform.eulerAngles.x;
                float originalZ = camera.transform.eulerAngles.z;
                camera.transform.Rotate(new Vector3(-originalX, 0, -originalZ)); //set X and Z to 0 to isolate movement to Y-axis
                camera.transform.Rotate(new Vector3(0, combinedDistanceMoved, 0)); //apply rotation to Y-axis
                camera.transform.Rotate(new Vector3(originalX, 0, originalZ)); //reapply X and Z rotations
            }
        }
        //**********************panning**********************
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            camera.transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
        }
        //**********************tapping**********************
        else if (Input.touchCount > 0)
        {
            BuildingManager.Instance.OnClick();
        }
    }

#if UNITY_EDITOR
#endif
}

