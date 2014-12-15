using UnityEngine;
using System.Collections;

public abstract class BaseStructure : MonoBehaviour, IDamageable {
    public Resource cost;
    public float buildTime;
    [HideInInspector]
    public float processTimeLeft;
    [HideInInspector]
    public bool isUnderConstruction;
    public int level;

    protected const float processUpdateInterval = 1.0f;
    protected delegate void DelayedOperation();
    protected DelayedOperation delayedOperation;
    protected Transform myTransform;
    protected GridComponent gridComponent;
    protected bool structureActive;
    protected int maxHealth;
    protected int health;
    protected bool isNew;

    #region IDamageable
    public abstract void Damage(int amount);
    #endregion

    protected virtual void Awake() {
        myTransform = transform;
        gridComponent = GetComponent<GridComponent>();
        if (gridComponent == null) { Debug.LogError("Structure requires GridComponent script"); }
    }

    protected virtual void OnEnable() {
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

    public virtual void Build() {
        gridComponent.AttachToGrid();
        StartCoroutine(StartDelayedOperation(BuildProcess, buildTime));
        isUnderConstruction = true;
        gridComponent.Move();
        isNew = true;
    }

    public void ContinueBuild(float timeLeft) {
        processTimeLeft = timeLeft;
        StartCoroutine(StartDelayedOperation(BuildProcess, processTimeLeft));
        isUnderConstruction = true;
    }

    protected GameObject timerDisplay;
    protected virtual IEnumerator StartDelayedOperation(DelayedOperation operation, float duration) {
        delayedOperation = operation;
        processTimeLeft = duration;
        timerDisplay = GUIManager.Instance.GetTimerDisplay(gameObject);
        while (processTimeLeft > 0) {
            timerDisplay.guiText.text = "Ready in " + processTimeLeft;
            processTimeLeft -= processUpdateInterval;
            yield return new WaitForSeconds(processUpdateInterval);
        }
        delayedOperation();
        yield return null;
    }

    private void BuildProcess() {
        Destroy(timerDisplay);
        Activate();
        isNew = false;
        isUnderConstruction = false;
    }
}
