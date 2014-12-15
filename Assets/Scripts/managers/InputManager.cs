using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    public GUIText DebugInfo;
    public Camera NGUICamera;
    public float dragTreshold; //minimum move distance (x+y) to be recognized as drag
    public float longTapTreshold; //minimum tap length to be recognized as LongTap

    public delegate void TapEvent(Vector3 tapPos);
    static public event TapEvent OnTap;

    public delegate void LongTapEvent(Vector3 tapPos);
    static public event LongTapEvent OnLongTap;

    public delegate void DragEvent(Vector2 deltaPos);
    static public event DragEvent OnDrag;

    public delegate void RotateEvent(float amount);
    static public event RotateEvent OnRotate;

    public delegate void PinchEvent(float amount);
    static public event PinchEvent OnPinch;

    private LayerMask uiLayerMask = 1 << 5;
    private State state;
    private enum State {
        IDLE, PINCHING, ROTATING, DRAGGING, TAPPED, LONGTAPPED
    }
    private float tapLength;
    private bool touchesClear;
    private bool tapOnGui;
    private bool acceptNewGesture;

    public void Update() {

        if (Input.touchCount == 0) { 
            touchesClear = true;
            acceptNewGesture = true;
        }
        if (state == State.IDLE && acceptNewGesture) { UpdateState(); }

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
            case State.LONGTAPPED:
                if (OnLongTap != null) {
                    OnLongTap(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
                }
                acceptNewGesture = false;
                tapLength = 0;
                state = State.IDLE;
                break;
            case State.TAPPED:
                if (OnTap != null) {
                    OnTap(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
                }
                tapLength = 0;
                state = State.IDLE;
                break;
            default:
                break;
        }
    }

    private void UpdateState() {
        if (Input.touchCount == 2) {
            touchesClear = false;
            tapLength = 0;
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
                //Debug.Log("a1 " + prevAngle + "  |  a2 " + angle);

                float ratio = Mathf.Abs(deltaAngle) / Mathf.Abs(deltaDistance);
                //Debug.Log("ratio " + ratio + "  |  " + deltaAngle + " / " + deltaDistance);

                if (ratio < 0.17f) { state = State.PINCHING; }
                else { state = State.ROTATING; }
            }
        }
        else if (Input.touchCount == 1) {
            Touch t = Input.GetTouch(0);

            if (touchesClear) {
                touchesClear = false;
                //check if touch is on GUI
                Ray inputRay = NGUICamera.ScreenPointToRay(t.position);
                RaycastHit hit;
                tapOnGui = Physics.Raycast(inputRay.origin, inputRay.direction, out hit, Mathf.Infinity, uiLayerMask);
            }

            //discard taps on GUI
            if (!tapOnGui) {
                tapLength += Time.deltaTime;
                float deltaMovement = Mathf.Abs(t.deltaPosition.x) + Mathf.Abs(t.deltaPosition.y);
                if (deltaMovement > dragTreshold) {
                    tapLength = 0;
                    state = State.DRAGGING;
                }
                else if (tapLength > longTapTreshold) {
                    state = State.LONGTAPPED;
                }
                else if (t.phase == TouchPhase.Ended) {
                    state = State.TAPPED;
                }
            }
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0)) {
            if (OnTap != null) {
                OnTap(Input.mousePosition);
            }
        }
#endif
    }
}