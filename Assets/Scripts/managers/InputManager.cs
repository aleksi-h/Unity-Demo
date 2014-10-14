using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager> {

    public delegate void TapEvent(Vector3 tapPos);
    public static event TapEvent OnTap;

    public delegate void DragEvent(Vector2 deltaPos);
    public static event DragEvent OnDrag;

    public delegate void RotateEvent(float amount);
    public static event RotateEvent OnRotate;

    public delegate void PinchEvent(float amount);
    public static event PinchEvent OnPinch;

    private State state;
    private enum State {
        IDLE, PINCHING, ROTATING, DRAGGING, TAPPING
    }

    void Update() {
        //if (UICamera.hoveredObject == null) {
            if (state == State.IDLE) { UpdateState(); }

            switch (state) {
                case State.PINCHING:
                    if (Input.touchCount < 2) { state = State.IDLE; }
                    else if (OnPinch != null) {
                        Touch t0 = Input.GetTouch(0);
                        Touch t1 = Input.GetTouch(1);
                        Vector2 t0PrevPos = t0.position - t0.deltaPosition;
                        Vector2 t1PrevPos = t1.position - t1.deltaPosition;
                        float prevFrameDeltaMag = (t0PrevPos - t1PrevPos).magnitude;
                        float deltaMag = (t0.position - t1.position).magnitude;
                        float deltaMagnitudeDiff = prevFrameDeltaMag - deltaMag;
                        OnPinch(deltaMagnitudeDiff);
                    }
                    break;
                case State.ROTATING:
                    if (Input.touchCount < 2) { state = State.IDLE; }
                    else if (OnRotate != null) {
                        Touch t0 = Input.GetTouch(0);
                        Touch t1 = Input.GetTouch(1);

                        //make sure that leftmost touch is t0
                        if (t0.position.x > t1.position.x) {
                            Touch aux = t0;
                            t0 = t1;
                            t1 = aux;
                        }
                        float t0DistanceMoved = t0.deltaPosition.y;
                        float t1DistanceMoved = -t1.deltaPosition.y;
                        float combinedDistanceMoved = t0DistanceMoved + t1DistanceMoved;
                        OnRotate(combinedDistanceMoved);
                    }
                    break;
                case State.DRAGGING:
                    if (Input.touchCount != 1) { state = State.IDLE; }
                    else if (OnDrag != null) { OnDrag(Input.GetTouch(0).deltaPosition); }
                    break;
                case State.TAPPING:
                    if (OnTap != null) {
                        Touch t = Input.GetTouch(0);
                        OnTap(new Vector3(t.position.x, t.position.y, 0));
                    }
                    state = State.IDLE;
                    break;
                default:
                    break;
            }
        //}
    }

    private void UpdateState() {
        if (Input.touchCount == 2) {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            if (t0.phase == TouchPhase.Moved && t1.phase == TouchPhase.Moved) {
                Vector2 t0PrevPos = t0.position - t0.deltaPosition;
                Vector2 t1PrevPos = t1.position - t1.deltaPosition;
                float prevDistance = Vector2.Distance(t0PrevPos, t1PrevPos);
                float distance = Vector2.Distance(t0.position, t1.position);
                float deltaDistance = prevDistance - distance;

                float prevAngle = Vector2.Angle(t0PrevPos - t1PrevPos, Vector2.up);
                float angle = Vector2.Angle(t0.position - t1.position, Vector2.up);
                float deltaAngle = Mathf.DeltaAngle(prevAngle, angle);
                Debug.Log("a1 " + prevAngle + "  |  a2 " + angle);

                float ratio = Mathf.Abs(deltaAngle) / Mathf.Abs(deltaDistance);
                Debug.Log("ratio " + ratio + "  |  " + deltaAngle + " / " + deltaDistance);

                if (ratio < 0.17f) { state = State.PINCHING; }
                else { state = State.ROTATING; }
            }
        }
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            state = State.DRAGGING;
        }
        else if (Input.touchCount == 1) {
            if (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).tapCount == 1) {
                state = State.TAPPING;
            }
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0)) {
            if (OnTap != null) {
                //OnTap(Input.mousePosition);
            }
        }
#endif
    }
}

