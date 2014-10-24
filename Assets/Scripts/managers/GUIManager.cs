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

    private LayerMask structureLayerMask = 1 << 10;
    private GameObject selectedStructure;
    private GameObject structureToBuild;
    private List<GameObject> timerDisplays;

    public override void Awake() {
        base.Awake();
        timerDisplays = new List<GameObject>(1);
    }

    public void Start() {
        HideAllGUIElements();
        ShowDefaultGUI();
        InputManager.OnTap += OnTap;
        InputManager.OnLongTap += OnLongTap;
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
        selectedStructure.GetComponent<BaseStructure>().ConfirmPosition();
        InputManager.OnTap += OnTap;
        ShowDefaultGUI();
    }

    //STRUCTURE PANEL
    public void OnClickMove() {
        if (Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position)) {
            BaseStructure structureBase = selectedStructure.GetComponent<BaseStructure>();
            structureBase.Move();
            ShowPlacementGUI(selectedStructure, structureBase.Type);
        }
    }
    public void OnClickUpgrade() {
        BuildingManager.Instance.UpgradeStructure(selectedStructure);
    }
    public void OnClickDelete() {
        BuildingManager.Instance.DeleteStructure(selectedStructure);
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

    public void ShowPlacementGUI(GameObject structure, StructureType type) {
        InputManager.OnTap -= OnTap;
        selectedStructure = structure;
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

    private void OnTap(Vector3 tapPos) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
            selectedStructure = hit.collider.gameObject;
            ShowStructureGUI();
        }
        else {
            ShowDefaultGUI();
        }
    }

    private void OnLongTap(Vector3 tapPos) {
        Debug.Log("onlongtap");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
            selectedStructure = hit.collider.gameObject;
            if (Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position)) {
                BaseStructure structureBase = selectedStructure.GetComponent<BaseStructure>();
                structureBase.Move();
                ShowPlacementGUI(selectedStructure, structureBase.Type);
            }
        }
    }
}
