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
    protected GridComponent gridComponent;
    protected bool structureActive;
    protected int maxHealth;
    protected int level;
    protected int health;

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    protected virtual void Awake() {
        myTransform = transform;
        gridComponent = GetComponent<GridComponent>();
        if (gridComponent == null) { Debug.LogError("Structure requires GridComponent script"); }
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

    public void Build() {
        gridComponent.AttachToGrid();
        StartDelayedOperation(BuildProcess, buildTime);
        gridComponent.Move();
    }

    public GridComponent getGridComponent() {
        return gridComponent;
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
