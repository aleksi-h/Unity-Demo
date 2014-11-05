using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IDamageable {
    public Resource cost;
    public float buildTime;

    protected Node node;
    protected const float processUpdateInterval = 1.0f;
    protected delegate void DelayedOperation();
    protected DelayedOperation delayedOperation;
    protected float processDuration;

    protected Transform myTransform;
    protected bool structureActive;
    protected int maxHealth;
    protected int level;
    protected int health;
    protected StructureType type;
    public StructureType Type {
        get { return type; }
    }

    private Vector3 positionBeforeMove;

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    public void Build() {
        Grid.Instance.BuildToNode(myTransform.position, gameObject);
        StartDelayedOperation(BuildProcess, buildTime);
    }

    public void AttachToNode(Node node) {
        this.node = node;
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
    }

    protected GameObject timerDisplay;
    protected virtual void StartDelayedOperation(DelayedOperation process, float duration) {
        delayedOperation = process;
        processDuration = duration;
        timerDisplay = GUIManager.Instance.GetTimerDisplay(gameObject);
        updateDisplayText();
        InvokeRepeating("UpdateDelayedOperation", 1.0f, processUpdateInterval);
    }

    protected virtual void UpdateDelayedOperation() {
        processDuration -= processUpdateInterval;
        if (processDuration <= 0) {
            CancelInvoke("UpdateDelayedOperation");
            delayedOperation();
        }
        else { updateDisplayText(); }
    }

    protected void updateDisplayText() {
        timerDisplay.guiText.text = "Ready in " + processDuration;
    }

    private void BuildProcess() {
        Destroy(timerDisplay);
        Activate();
    }
}
