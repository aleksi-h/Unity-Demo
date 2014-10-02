using UnityEngine;
using System.Collections;

public class Builder:MonoBehaviour {
    private int duration;
    private Vector3 position;
    private GameObject structure;
    private GameObject upgradedStructure;
    private GameObject display;
    private Vector3 displayPosition;
    private string displayText;

	void Start () {
	
	}
	
	void Update () {
	
	}

    public void UpgradeStructure(GameObject selectedStructure, int upgradeDuration, GameObject nextLevelPrefab){
        duration = upgradeDuration;
        position = selectedStructure.transform.position;
        structure = selectedStructure;
        upgradedStructure = nextLevelPrefab;

        displayText = "upgrading ";
        displayPosition = Camera.main.WorldToViewportPoint(position);
        displayPosition.x = displayPosition.x - 0.04f;
        displayPosition.y = displayPosition.y - 0.02f;

        display = new GameObject("Display");
        display.AddComponent<GUIText>();
        display.transform.position = displayPosition;
        display.guiText.text = displayText+duration;

        InvokeRepeating("CheckProgress", 0, 1.0F);
    }

    private void CheckProgress()
    {
        if (duration <= 0) { UpgradeFinished(); }
        else
        {
            display.guiText.text = displayText + duration;
        }
        duration--;
    }

    private void UpgradeFinished()
    {
        CancelInvoke("CheckProgress");
        Destroy(display);
        UnityEngine.Object.Destroy(structure);
        UnityEngine.Object.Instantiate(upgradedStructure, position, Quaternion.identity);
    }
}
