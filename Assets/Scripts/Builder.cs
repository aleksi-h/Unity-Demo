using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour
{
    private int duration;
    private GameObject structure;
    private GameObject upgradedStructure;
    private GameObject display;
    private Vector3 displayPosition;
    private string displayText;

    public void Update()
    {
        if (structure != null && transform.position != structure.transform.position)
        {
            transform.position = structure.transform.position;
            SetDisplayPosition(transform.position);
        }
    }

    private void SetDisplayPosition(Vector3 position)
    {
        displayPosition = Camera.main.WorldToViewportPoint(position);
        displayPosition.x = displayPosition.x - 0.04f;
        displayPosition.y = displayPosition.y - 0.02f;
        display.transform.position = displayPosition;
    }

    private void addDisplay()
    {
        display = new GameObject("Display");
        display.transform.parent = transform;
        display.AddComponent<GUIText>();
        display.guiText.text = displayText + duration;
        SetDisplayPosition(transform.position);
    }

    public void BuildStructure(GameObject structuretoBuild, int buildDuration)
    {
        duration = buildDuration;
        transform.position = structuretoBuild.transform.position;
        structure = structuretoBuild;
        displayText = "building ";
        addDisplay();

        InvokeRepeating("CheckBuildProgress", 0, 1.0F);
    }

    public void UpgradeStructure(GameObject target, int upgradeDuration, GameObject nextLevelPrefab)
    {
        duration = upgradeDuration;
        transform.position = target.transform.position;
        structure = target;
        upgradedStructure = nextLevelPrefab;
        displayText = "upgrading ";
        addDisplay();

        InvokeRepeating("CheckUpgradeProgress", 0, 1.0F);
    }

    private void CheckBuildProgress()
    {
        if (duration <= 0) { BuildFinished(); }
        else { display.guiText.text = displayText + duration; }
        duration--;
    }

    private void CheckUpgradeProgress()
    {
        if (duration <= 0) { UpgradeFinished(); }
        else { display.guiText.text = displayText + duration; }
        duration--;
    }

    private void BuildFinished()
    {
        CancelInvoke("CheckBuildProgress");
        Destroy(display);
        structure.GetComponent<BaseStructure>().Activate();
        Destroy(gameObject);
    }

    private void UpgradeFinished()
    {
        CancelInvoke("CheckUpgradeProgress");
        Destroy(display);
        Destroy(structure);
        GameObject upgraded = (GameObject)Instantiate(upgradedStructure, transform.position, Quaternion.identity);
        upgraded.GetComponent<BaseStructure>().Activate();
        Destroy(gameObject);
    }
}
