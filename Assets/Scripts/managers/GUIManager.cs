using UnityEngine;
using System.Collections.Generic;

public class GUIManager : Singleton<GUIManager> {
    public Transform gridRoot;
    public UIRoot uiRoot;
    public Camera nguiCamera;
    
    public GameObject timerDisplay;
    public GameObject oldBuildPanel;
    public GameObject confirmBuildPanel;
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
        UIEventListener.Get(acceptButton).onClick += OnClickConfirmMovement;
        UIEventListener.Get(cancelButton).onClick += OnClickCancelMovement;
        UIEventListener.Get(sawmillButton).onClick += showBuyMenu;
        UIEventListener.Get(fieldButton).onClick += showBuyMenu;
        UIEventListener.Get(storageButton).onClick += showBuyMenu;
        UIEventListener.Get(hutButton).onClick += showBuyMenu;
        UIEventListener.Get(buyButton).onClick += onClickBuy;
    }

    //avoid null reference when changing prefab after upgrade
    public void UpgradeFinished(GameObject oldGo, GameObject newGo) {
        if (selectedStructure == oldGo) {
            selectedStructure = newGo;
        }
    }

    //BUILD MENU
    public GameObject buildButton;
    public GameObject buildMenu;
    public GameObject buildMenuGrid;
    public GameObject sawmillButton;
    public GameObject fieldButton;
    public GameObject storageButton;
    public GameObject hutButton;

    private void showBuildMenu(GameObject button) {
        HideAllGUIElements();
        NGUITools.SetActive(buildMenu, true);
    }

    public void showBuildDialog(GameObject obj) {
        HideAllGUIElements();
        Resource cost = obj.GetComponent<BaseStructure>().cost;
        confirmButtonLabel.GetComponent<UILabel>().text = "Wood " + cost.wood + "\nFood " + cost.food;
        NGUITools.SetActive(confirmBuildPanel, true);
    }

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

    //BUY MENU
    public GameObject buyMenu;
    public GameObject buyButton;
    public GameObject priceLabel;
    private void showBuyMenu(GameObject button) {
        HideAllGUIElements();
        if (button == sawmillButton) {
            structureToBuild = BuildingManager.Instance.sawmill;
        }
        else if (button == fieldButton) {
            structureToBuild = BuildingManager.Instance.field;
        }
        else if (button == storageButton) {
            structureToBuild = BuildingManager.Instance.storage;
        }
        else if (button == hutButton) {
            structureToBuild = BuildingManager.Instance.hut;
        }

        if (structureToBuild != null) {
            Resource price = structureToBuild.GetComponent<BaseStructure>().cost;
            priceLabel.GetComponent<UILabel>().text = price.wood + "W " + price.food + "F";
        }
        NGUITools.SetActive(buyMenu, true);
    }
    private void onClickBuy(GameObject button) {
        BuildingManager.instance.BuildStructure(structureToBuild);
    }

    //CONFIRM BUILD PANEL
    public void OnClickConfirmBuild() {
        BuildingManager.instance.BuildStructure(structureToBuild);
    }
    public void OnClickCancel() {
        ShowDefaultMenu();
    }

    //MOVEMENT MENU
    public GameObject confirmPlacementMenu;
    public GameObject acceptButton;
    public GameObject cancelButton;

    private void OnClickConfirmMovement(GameObject go) {
        Debug.Log(go);
        selectedStructure.GetComponent<BaseStructure>().FinishMove();
        InputManager.OnTap += OnTap;
        ShowDefaultMenu();
    }
    private void OnClickCancelMovement(GameObject go) {
        selectedStructure.GetComponent<BaseStructure>().CancelMove();
        InputManager.OnTap += OnTap;
        ShowDefaultMenu();
    }

    //CONTEXT MENU
    public GameObject contextMenu;
    public GameObject contextMenuGrid;
    public GameObject upgradeButtonPrefab;
    public GameObject removeButtonPrefab;

    private void OnClickUpgrade(GameObject go) {
        BuildingManager.Instance.UpgradeStructure(selectedStructure);
    }
    private void OnClickRemove(GameObject go) {
        BuildingManager.Instance.DeleteStructure(selectedStructure);
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
            UIEventListener.Get(btn).onClick += OnClickUpgrade;//TODO remove ref somewhere?
        }
        if (selectedStructure.GetInterface<IRemovable>() != null) {
            GameObject btn = NGUITools.AddChild(contextMenuGrid, removeButtonPrefab);
            UIEventListener.Get(btn).onClick += OnClickRemove;
        }
        contextMenuGrid.GetComponent<UIGrid>().Reposition();
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

    public void ShowDefaultMenu() {
        HideAllGUIElements();
        NGUITools.SetActive(oldBuildPanel, true);
        NGUITools.SetActive(buildButton, true);
    }


    public void ShowPlacementGUI(GameObject structure, StructureType type) {
        InputManager.OnTap -= OnTap;
        selectedStructure = structure;
        HideAllGUIElements();
        Vector3 offset = new Vector3(0, 15, 0);
        FollowGameObjectNGUI followScript = confirmPlacementMenu.GetComponent<FollowGameObjectNGUI>();
        followScript.setOffset(offset);
        followScript.SetTarget(selectedStructure);
        NGUITools.SetActive(confirmPlacementMenu, true);
        Grid.Instance.HighLightValidNodes(type);
    }


    private void HideAllGUIElements() {
        //stop following a target when hidden
        contextMenu.GetComponent<FollowGameObjectNGUI>().SetTarget(null);
        confirmPlacementMenu.GetComponent<FollowGameObjectNGUI>().SetTarget(null);

        //clear items from context menu grid
        int menuItemCount = contextMenuGrid.transform.childCount;
        for (int i = 0; i < menuItemCount; i++) {
            Transform child = contextMenuGrid.transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        Grid.Instance.HideHighlight();
        NGUITools.SetActive(buildButton, false);

        //hide all menus
        NGUITools.SetActive(buyMenu, false);
        NGUITools.SetActive(confirmPlacementMenu, false);
        NGUITools.SetActive(buildMenu, false);
        NGUITools.SetActive(contextMenu, false);
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
