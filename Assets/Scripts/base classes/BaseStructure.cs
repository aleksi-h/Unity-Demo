using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IDamageable {
    public Resource cost;
    public float buildTime;

    protected delegate void LongProcess();
    protected LongProcess longProcess;
    protected float processDuration;
    protected const float processUpdateInterval = 1.0f;
    protected GameObject timerDisplay;

    protected Transform myTransform;
    protected bool structureActive;
    protected bool onTapRegistered;
    protected int maxHealth;
    protected int level;
    protected int health;
    protected StructureType type;
    public StructureType Type {
        get { return type; }
    }

    private LayerMask groundLayerMask = 1 << 11;
    private Vector3 positionBeforeMove;

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    public void Build() {
        Grid.Instance.BuildToNode(myTransform.position, type);
        StartLongProcess(BuildProcess, buildTime);
    }

    //start receiving onTap events
    public void Move() {
        InputManager.OnTap += OnTap;
        onTapRegistered = true;
        positionBeforeMove = myTransform.position;
    }

    //stop receiving onTap events and go back to previous position
    public void CancelMove() {
        InputManager.OnTap -= OnTap;
        onTapRegistered = false;
        if (!myTransform.position.Equals(positionBeforeMove)) {
            Grid.Instance.RemoveFromNode(myTransform.position);
            Grid.Instance.BuildToNode(positionBeforeMove, type);
            myTransform.position = positionBeforeMove;
        }
    }

    public void FinishMove() {
        InputManager.OnTap -= OnTap;
        onTapRegistered = false;
    }

    protected virtual void Awake() {
        myTransform = transform;
    }

    protected virtual void Start() {
    }

    protected virtual void Update() {
    }

    public virtual void Activate() {
        structureActive = true;
    }

    protected virtual void OnDestroy() {
        if (onTapRegistered) {
            InputManager.OnTap -= OnTap;
        }
    }

    protected virtual void StartLongProcess(LongProcess process, float duration) {
        longProcess = process;
        processDuration = duration;
        timerDisplay = GUIManager.Instance.AddTimerDisplay(gameObject, "ready in " + processDuration);
        InvokeRepeating("UpdateLongProcess", 1.0f, processUpdateInterval);
    }

    protected virtual void UpdateLongProcess() {
        processDuration -= processUpdateInterval;
        if (processDuration <= 0) {
            CancelInvoke("UpdateLongProcess");
            longProcess();
        }
        else {
            GUIManager.Instance.UpdateTimerDisplay(timerDisplay, "ready in " + processDuration);
        }

    }

    private void BuildProcess() {
        GUIManager.Instance.RemoveTimerDisplay(timerDisplay);
        Activate();
    }

    //move to selected node
    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
            Vector3 newPos = Grid.Instance.GetNearestValidNode(myTransform.position, hit.point, type);
            if (!myTransform.position.Equals(newPos)) {
                Grid.Instance.RemoveFromNode(myTransform.position);
                Grid.Instance.BuildToNode(newPos, type);
                myTransform.position = newPos;
            }
        }
    }
}
