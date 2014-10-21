using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {
    private int duration;
    private GameObject structure;
    private GameObject upgradedStructure;
    private GameObject display;
    private Vector3 displayPosition;
    private string displayText;
    private Transform myTransform;

    public void Awake() {
        //caching transform to avoid the lookup each time
        myTransform = transform;
    }

    public void Update() {
        if (structure != null) {
            myTransform.position = structure.transform.position;
            SetDisplayPosition(myTransform.position);
        }
    }

    public void BuildStructure(GameObject structuretoBuild, int buildDuration) {
        duration = buildDuration;
        myTransform.position = structuretoBuild.transform.position;
        structure = structuretoBuild;
        addDisplay("building ");

        InvokeRepeating("CheckBuildProgress", 0, 1.0F);
    }

    public void UpgradeStructure(GameObject target, int upgradeDuration, GameObject nextLevelPrefab) {
        duration = upgradeDuration;
        myTransform.position = target.transform.position;
        structure = target;
        upgradedStructure = nextLevelPrefab;
        addDisplay("upgrading ");

        InvokeRepeating("CheckUpgradeProgress", 0, 1.0F);
    }

    private void SetDisplayPosition(Vector3 position) {
        displayPosition = Camera.main.WorldToViewportPoint(position);
        displayPosition.x -= 0.04f;
        displayPosition.y -= 0.02f;
        display.transform.position = displayPosition;
    }

    private void addDisplay(string text) {
        display = new GameObject("Display");
        display.transform.parent = myTransform;
        display.AddComponent<GUIText>();
        displayText = text;
        display.guiText.text = displayText + duration;
        SetDisplayPosition(myTransform.position);
    }

    private void CheckBuildProgress() {
        if (duration <= 0) { BuildFinished(); }
        else { display.guiText.text = displayText + duration; }
        duration--;
    }

    private void CheckUpgradeProgress() {
        if (duration <= 0) { UpgradeFinished(); }
        else { display.guiText.text = displayText + duration; }
        duration--;
    }

    private void BuildFinished() {
        CancelInvoke("CheckBuildProgress");
        Destroy(display);
        structure.GetComponent<BaseStructure>().Activate();
        Destroy(gameObject);
    }

    private void UpgradeFinished() {
        CancelInvoke("CheckUpgradeProgress");
        Destroy(display);
        if (structure.ImplementsInterface<IRemovable>()) {
            structure.GetInterface<IRemovable>().Remove();
        }
        else { Destroy(structure); }
        GameObject upgraded = (GameObject)Instantiate(upgradedStructure, myTransform.position, Quaternion.identity);
        upgraded.GetComponent<BaseStructure>().Activate();
        Destroy(gameObject);
    }
}