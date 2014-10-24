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
    protected int maxHealth;
    protected int level;
    protected int health;
    protected StructureType type;
    public StructureType Type {
        get { return type; }
    }

    private LayerMask groundLayerMask = 1 << 11;

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    public void Build() {
        StartLongProcess(BuildProcess, buildTime);
    }

    public void Move() {
        InputManager.OnTap += OnTap;
        Grid.Instance.RemoveFromNode(myTransform.position);
    }

    public void ConfirmPosition() {
        InputManager.OnTap -= OnTap;
        Grid.Instance.BuildToNode(myTransform.position, type);
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

    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
            myTransform.position = Grid.Instance.GetNearestValidNode(myTransform.position, hit.point, type);
        }
    }
}
