using UnityEngine;
using System.Collections.Generic;

public class GUIManager : Singleton<GUIManager> {
    public Transform gridRoot;
    public UIRoot uiRoot;
    public Camera nguiCamera;
    
    public GameObject timerDisplay;
    public GameObject oldBuildPanel;
    public GameObject confirmBuildPanel;
    public GameObject confirmPlacementPanel;

    //BUILD MENU
    public GameObject buildButton;
    public GameObject buildPanel;

    //CONTEXT MENU
    public GameObject oldCtxMenu;
    public GameObject contextMenu;
    public GameObject contextMenuGrid;
    public GameObject upgradeButtonPrefab;
    public GameObject removeButtonPrefab;

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
        ShowDefaultMenu();
        InputManager.OnTap += OnTap;
        InputManager.OnLongTap += OnLongTap;
        UIEventListener.Get(buildButton).onClick += showBuildMenu;
    }

    //avoid null reference when changing prefab
    public void UpgradeFinished(GameObject oldGo, GameObject newGo) {
        if (selectedStructure == oldGo) {
            selectedStructure = newGo;
        }
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
        ShowDefaultMenu();
    }

    //CONFIRM PLACEMENT PANEL
    public void OnClickConfirmPlacement() {
        selectedStructure.GetComponent<BaseStructure>().ConfirmPosition();
        InputManager.OnTap += OnTap;
        ShowDefaultMenu();
    }

    //STRUCTURE PANEL
    public void OnClickMove() {
        if (Grid.Instance.IsBuildingMoveable(selectedStructure.transform.position)) {
            BaseStructure structureBase = selectedStructure.GetComponent<BaseStructure>();
            structureBase.Move();
            ShowPlacementGUI(selectedStructure, structureBase.Type);
        }
    }
    public void OnClickUpgrade(GameObject go) {
        BuildingManager.Instance.UpgradeStructure(selectedStructure);
    }
    public void OnClickRemove(GameObject go) {
        BuildingManager.Instance.DeleteStructure(selectedStructure);
    }


    public GameObject AddTimerDisplay(GameObject caller, string text) {
        GameObject display = (GameObject)Instantiate(timerDisplay, caller.transform.position, Quaternion.identity);
        display.transform.parent = transform;
        display.guiText.text = text;
        display.GetComponent<FollowGameObject>().FollowObject(caller);
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

    public void ShowDefaultMenu() {
        HideAllGUIElements();
        NGUITools.SetActive(oldBuildPanel, true);
        NGUITools.SetActive(buildButton, true);
    }

    public void ShowContextMenu() {
        HideAllGUIElements();

        Vector3 offset = new Vector3(0, 15, 0);
        FollowGameObjectNGUI followScript = contextMenu.GetComponent<FollowGameObjectNGUI>();
        followScript.setOffset(offset);
        followScript.SetTarget(selectedStructure);

        NGUITools.SetActive(contextMenu, true);
        if (selectedStructure.ImplementsInterface<IUpgradeable>()) {
            GameObject btn = NGUITools.AddChild(contextMenuGrid, upgradeButtonPrefab);
            UIEventListener.Get(btn).onClick += OnClickUpgrade;
        }
        if (selectedStructure.GetInterface<IRemovable>() != null) {
            GameObject btn = NGUITools.AddChild(contextMenuGrid, removeButtonPrefab);
            UIEventListener.Get(btn).onClick += OnClickRemove;
        }
        contextMenuGrid.GetComponent<UIGrid>().Reposition();
    }

    public void ShowPlacementGUI(GameObject structure, StructureType type) {
        InputManager.OnTap -= OnTap;
        selectedStructure = structure;
        HideAllGUIElements();
        NGUITools.SetActive(confirmPlacementPanel, true);
        Grid.Instance.HighLightValidNodes(type);
    }

    private void showBuildMenu(GameObject go) {
        NGUITools.SetActive(buildPanel, true);
    }

    private void HideAllGUIElements() {
        Grid.Instance.HideHighlight();

        //stop following a target when hidden
        contextMenu.GetComponent<FollowGameObjectNGUI>().SetTarget(null);

        //clear the context menu
        int menuItemCount = contextMenuGrid.transform.childCount;
        for (int i = 0; i < menuItemCount; i++) {
            Transform child = contextMenuGrid.transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        NGUITools.SetActive(buildButton, false);
        //hide all panels
        NGUITools.SetActive(buildPanel, false);
        NGUITools.SetActive(oldCtxMenu, false);
        NGUITools.SetActive(contextMenu, false);
        NGUITools.SetActive(confirmPlacementPanel, false);
        NGUITools.SetActive(oldBuildPanel, false);
        NGUITools.SetActive(confirmBuildPanel, false);
    }

    private void OnTap(Vector3 tapPos) {
        Debug.Log("OnTap");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
            selectedStructure = hit.collider.gameObject;
            ShowContextMenu();
        }
        else {
            ShowDefaultMenu();
        }
    }

    private void OnLongTap(Vector3 tapPos) {
        Debug.Log("OnLongTap");
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
