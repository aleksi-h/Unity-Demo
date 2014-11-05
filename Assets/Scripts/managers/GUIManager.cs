using UnityEngine;
using System.Collections.Generic;

public class GUIManager : Singleton<GUIManager> {
    public Transform gridRoot;
    public UIRoot uiRoot;
    public Camera nguiCamera;
    public GameObject timerDisplay;

    private LayerMask structureLayerMask = 1 << 10;
    private GameObject selectedStructure;

    public override void Awake() {
        base.Awake();
    }

    public void Start() {
        HideAllGUIElements();
        ShowDefaultMenu();
        InputManager.OnTap += OnTap;
        //InputManager.OnLongTap += OnLongTap;
        UIEventListener.Get(buildButton).onClick += ShowBuildMenu;
        UIEventListener.Get(closeButton).onClick += OnClickClose;
        UIEventListener.Get(acceptButton).onClick += OnClickConfirmMovement;
        UIEventListener.Get(cancelButton).onClick += OnClickCancelMovement;
        UIEventListener.Get(sawmillButton).onClick += showBuyMenu;
        UIEventListener.Get(fieldButton).onClick += showBuyMenu;
        UIEventListener.Get(storageButton).onClick += showBuyMenu;
        UIEventListener.Get(hutButton).onClick += showBuyMenu;
        UIEventListener.Get(buyButton1).onClick += onClickBuy1;
        UIEventListener.Get(buyButton2).onClick += onClickBuy2;
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
    public GameObject closeButton;
    public GameObject sawmillButton;
    public GameObject fieldButton;
    public GameObject storageButton;
    public GameObject hutButton;

    private void ShowBuildMenu(GameObject button) {
        HideAllGUIElements();
        NGUITools.SetActive(buildMenu, true);
    }

    private void OnClickClose(GameObject button) {
        ShowDefaultMenu();
    }


    //SETTINGS MENU
    public GameObject settingsButton;



    //BUY MENU
    public GameObject buyMenu;
    public GameObject structureIcon;
    public GameObject buyButton1;
    public GameObject priceLabel1;
    public GameObject buyButton2;
    public GameObject priceLabel2;
    private GameObject structureToBuild;
    private void showBuyMenu(GameObject button) {
        HideAllGUIElements();

        if (button == sawmillButton) {
            structureIcon.GetComponent<UISprite>().spriteName = "icon_sawmill";
            structureToBuild = BuildingManager.Instance.sawmill;
        }
        else if (button == fieldButton) {
            structureIcon.GetComponent<UISprite>().spriteName = "icon_field";
            structureToBuild = BuildingManager.Instance.field;
        }
        else if (button == storageButton) {
            structureIcon.GetComponent<UISprite>().spriteName = "icon_storage";
            structureToBuild = BuildingManager.Instance.storage;
        }
        else if (button == hutButton) {
            structureIcon.GetComponent<UISprite>().spriteName = "icon_hut";
            structureToBuild = BuildingManager.Instance.hut;
        }

        InvokeRepeating("SetBuyMenuContent", 0, 0.5f);
        NGUITools.SetActive(buyMenu, true);
    }

    private void SetBuyMenuContent() {
        if (structureToBuild != null) {
            Resource cost = structureToBuild.GetComponent<BaseStructure>().cost;
            Resource costInCurrency = Utils.ConvertResourcesToCurrency(cost);
            priceLabel1.GetComponent<UILabel>().text = cost.wood + "W\n" + cost.food + "F";
            priceLabel2.GetComponent<UILabel>().text = costInCurrency.currency + "Curr";

            bool canAfford = BuildingManager.Instance.CanAffordStructure(structureToBuild, cost);
            buyButton1.GetComponent<UIImageButton>().isEnabled = canAfford;

            canAfford = BuildingManager.Instance.CanAffordStructure(structureToBuild, costInCurrency);
            buyButton2.GetComponent<UIImageButton>().isEnabled = canAfford;
        }
    }

    private void onClickBuy1(GameObject button) {
        Resource cost = structureToBuild.GetComponent<BaseStructure>().cost;
        BuildingManager.instance.BuildStructure(structureToBuild, cost);
    }

    private void onClickBuy2(GameObject button) {
        Resource cost = structureToBuild.GetComponent<BaseStructure>().cost;
        Resource costInCurrency = Utils.ConvertResourcesToCurrency(cost);
        BuildingManager.instance.BuildStructure(structureToBuild, costInCurrency);
    }


    //MOVEMENT MENU
    public GameObject confirmPlacementMenu;
    public GameObject acceptButton;
    public GameObject cancelButton;

    private void OnClickConfirmMovement(GameObject button) {
        Grid.Instance.FinishMove();
        InputManager.OnTap += OnTap;
        ShowDefaultMenu();
    }
    private void OnClickCancelMovement(GameObject button) {
        Grid.Instance.CancelMove();
        InputManager.OnTap += OnTap;
        ShowDefaultMenu();
    }

    //CONTEXT MENU
    public GameObject contextMenu;
    public GameObject contextMenuGrid;
    public GameObject upgradeButtonPrefab;
    public GameObject removeButtonPrefab;
    public GameObject addworkerButtonPrefab;
    public GameObject removeworkerButtonPrefab;

    private void OnClickUpgrade(GameObject go) {
        BuildingManager.Instance.UpgradeStructure(selectedStructure);
        ShowDefaultMenu();
    }
    private void OnClickRemove(GameObject go) {
        BuildingManager.Instance.RemoveStructure(selectedStructure);
        ShowDefaultMenu();
    }
    private void OnClickAddworker(GameObject go) {
        selectedStructure.GetInterface<IEmployer>().AddWorker();
        ShowContextMenu();
    }
    private void OnClickRemoveworker(GameObject go) {
        selectedStructure.GetInterface<IEmployer>().FreeWorker();
        ShowContextMenu();
    }
    public void ShowContextMenu() {
        HideAllGUIElements();

        Vector3 offset = new Vector3(0, 15, 0);
        FollowGameObjectNGUI followScript = contextMenu.GetComponent<FollowGameObjectNGUI>();
        followScript.setOffset(offset);
        followScript.SetTarget(selectedStructure);
        NGUITools.SetActive(contextMenu, true);

        if (selectedStructure.ImplementsInterface<IUpgradeable>()) {
            upgradeButton = NGUITools.AddChild(contextMenuGrid, upgradeButtonPrefab);
            UIEventListener.Get(upgradeButton).onClick += OnClickUpgrade;//TODO ref needs to be removed somewhere?
        }

        if (selectedStructure.ImplementsInterface<IEmployer>()) {
            addworkerButton = NGUITools.AddChild(contextMenuGrid, addworkerButtonPrefab);
            removeworkerButton = NGUITools.AddChild(contextMenuGrid, removeworkerButtonPrefab);
            UIEventListener.Get(addworkerButton).onClick += OnClickAddworker;
            UIEventListener.Get(removeworkerButton).onClick += OnClickRemoveworker;
        }

        if (selectedStructure.ImplementsInterface<IRemovable>()) {
            removeButton = NGUITools.AddChild(contextMenuGrid, removeButtonPrefab);
            UIEventListener.Get(removeButton).onClick += OnClickRemove;}

        SetContextMenuContent();
        InvokeRepeating("SetContextMenuContent",0,0.5f);
        contextMenuGrid.GetComponent<UIGrid>().Reposition();
    }

    private GameObject upgradeButton;
    private GameObject removeButton;
    private GameObject addworkerButton;
    private GameObject removeworkerButton;

    private void SetContextMenuContent() {
        IUpgradeable upgradeable = selectedStructure.GetInterface<IUpgradeable>();
        if (upgradeable != null) {
            bool canUpgrade = BuildingManager.Instance.CanUpgradeStructure(upgradeable);
            upgradeButton.GetComponent<UIImageButton>().isEnabled = canUpgrade;
        }

        IEmployer employer = selectedStructure.GetInterface<IEmployer>();
        if (employer != null) {
            bool canAddWorkers = (ResourceManager.Instance.HasFreeWorkers(1) && employer.MaxWorkerCount > employer.Workers.Count);
            addworkerButton.GetComponent<UIImageButton>().isEnabled = canAddWorkers;

            bool canRemoveWorkers = (employer.MinWorkerCount < employer.Workers.Count);
            removeworkerButton.GetComponent<UIImageButton>().isEnabled = canRemoveWorkers;
        }

        if (selectedStructure.ImplementsInterface<IRemovable>()) {
            bool canRemoveBuilding = BuildingManager.Instance.CanRemoveStructure(selectedStructure);
            removeButton.GetComponent<UIImageButton>().isEnabled = canRemoveBuilding;
        }
    }


    public GameObject GetTimerDisplay(GameObject caller) {
        GameObject display = (GameObject)Instantiate(timerDisplay, caller.transform.position, Quaternion.identity);
        display.transform.parent = transform;
        display.GetComponent<FollowGameObject>().FollowObject(caller);
        return display;
    }

    public void ShowDefaultMenu() {
        HideAllGUIElements();
        NGUITools.SetActive(buildButton, true);
        NGUITools.SetActive(settingsButton, true);
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

    public void StructureUpgraded(GameObject oldGo, GameObject newGo) {
        if (oldGo == selectedStructure) {
            selectedStructure = newGo;
        }
        if (NGUITools.GetActive(confirmPlacementMenu)) {
            confirmPlacementMenu.GetComponent<FollowGameObjectNGUI>().SetTarget(selectedStructure);
        }
    }

    private void HideAllGUIElements() {
        //stop updating menus
        CancelInvoke();

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
        //hide buttons
        NGUITools.SetActive(buildButton, false);
        NGUITools.SetActive(settingsButton, false);

        //hide menus
        NGUITools.SetActive(buyMenu, false);
        NGUITools.SetActive(confirmPlacementMenu, false);
        NGUITools.SetActive(buildMenu, false);
        NGUITools.SetActive(contextMenu, false);
    }

    private void OnTap(Vector3 tapPos) {
        Debug.Log("OnTap");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1100, structureLayerMask)) {
            selectedStructure = hit.collider.gameObject;
            ShowContextMenu();
        }
        else { ShowDefaultMenu(); }
    }
}
