using UnityEngine;
using System.Collections.Generic;

public class GUIManager : Singleton<GUIManager> {
    public Transform GridRoot;

    public GameObject timerDisplay;
    public GameObject buildPanel;
    public GameObject confirmBuildPanel;
    public GameObject structurePanel;
    public GameObject confirmPlacementPanel;

    public GameObject confirmButtonLabel;

    private GameObject structureToBuild;
    private List<GameObject> timerDisplays;

    public override void Awake() {
        base.Awake();
        timerDisplays = new List<GameObject>(1);
    }

    public void Start() {
        HideAllGUIElements();
        ShowDefaultGUI();
    }

    //BUILD PANEL
    public void OnClickStorage() {
        structureToBuild = BuildingManager.instance.storage;
        showBuildDialog(structureToBuild);
    }
    public void OnClickSawmill() {
        structureToBuild = BuildingManager.instance.sawmill;
        showBuildDialog(structureToBuild);
    }
    public void OnClickHut() {
        structureToBuild = BuildingManager.instance.hut;
        showBuildDialog(structureToBuild);
    }
    public void OnClickField() {
        structureToBuild = BuildingManager.instance.field;
        showBuildDialog(structureToBuild);
    }

    //CONFIRM BUILD PANEL
    public void OnClickConfirmBuild() {
        BuildingManager.instance.BuildStructure(structureToBuild);
    }
    public void OnClickCancel() {
        ShowDefaultGUI();
    }

    //CONFIRM PLACEMENT PANEL
    public void OnClickConfirmPlacement() {
        BuildingManager.Instance.ConfirmPosition();
    }

    //STRUCTURE PANEL
    public void OnClickMove() {
        BuildingManager.Instance.MoveStructure();
    }
    public void OnClickUpgrade() {
        BuildingManager.Instance.UpgradeStructure();
    }
    public void OnClickDelete() {
        BuildingManager.Instance.DeleteStructure();
    }


    public GameObject AddTimerDisplay(GameObject caller, string text) {
        GameObject display = (GameObject)Instantiate(timerDisplay, caller.transform.position, Quaternion.identity);
        display.transform.parent = transform;
        display.guiText.text = text;
        display.GetComponent<TimerDisplay>().FollowObject(caller);
        timerDisplays.Add(display);
        return display;
    }

    public void UpdateTimerDisplay(GameObject display, string text) {
        display.guiText.text = text;
    }

    public void RemoveTimerDisplay(GameObject display) {
        timerDisplays.Remove(display);
        Destroy(display);
    }

    public void showBuildDialog(GameObject obj) {
        HideAllGUIElements();
        Resource cost = obj.GetComponent<BaseStructure>().cost;
        confirmButtonLabel.GetComponent<UILabel>().text = "Wood "+cost.wood + "\nFood "+cost.food;
        NGUITools.SetActive(confirmBuildPanel, true);
    }

    public void ShowDefaultGUI() {
        HideAllGUIElements();
        NGUITools.SetActive(buildPanel, true);
    }

    public void ShowStructureGUI() {
        //TODO get a ref to structure & enable/disable buttons based on availability eg. 0 workers, disable "add worker" button
        HideAllGUIElements();
        NGUITools.SetActive(structurePanel, true);
    }

    public void ShowPlacementGUI(StructureType type) {
        HideAllGUIElements();
        NGUITools.SetActive(confirmPlacementPanel, true);
        Grid.Instance.HighLightValidNodes(type);
    }

    private void HideAllGUIElements() {
        Grid.Instance.HideHighlight();
        NGUITools.SetActive(confirmPlacementPanel, false);
        NGUITools.SetActive(buildPanel, false);
        NGUITools.SetActive(confirmBuildPanel, false);
        NGUITools.SetActive(structurePanel, false);
    }
}
