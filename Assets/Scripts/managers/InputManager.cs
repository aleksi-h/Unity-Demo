using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager>
{
    public delegate void TapEvent(Vector3 tapPos);
    public static event TapEvent OnTap;

    public delegate void DragEvent(Touch t);
    public static event DragEvent OnDrag;

    public delegate void RotateEvent(Touch t1, Touch t2);
    public static event RotateEvent OnRotate;

    private bool idle;

    void Update()
    {
        //rotate
        if (Input.touchCount == 2)
        {
            idle = false;
            if (OnRotate != null)
            {
                OnRotate(Input.GetTouch(0), Input.GetTouch(1));
            }
        }
        //drag
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            idle = false;
            if (OnDrag != null)
            {
                OnDrag(Input.GetTouch(0));
            }
        }
        //tap
        else if (Input.touchCount == 1 && idle)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).tapCount == 1)
            {
                if (OnTap != null)
                {
                    Touch t = Input.GetTouch(0);
                    OnTap(new Vector3(t.position.x, t.position.y, 0));
                }
            }
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0))
        {
            if (OnTap != null)
            {
                //OnTap(Input.mousePosition);
            }
        }
#endif

        if (Input.touchCount == 0) { idle = true; }
    }

#if UNITY_EDITOR
#endif
}

