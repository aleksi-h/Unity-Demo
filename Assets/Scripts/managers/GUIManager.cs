using UnityEngine;
using System.Collections;

/*
 * A singleton GUI manager that persists between scenes
 * 
 */
public class GUIManager : Singleton<GUIManager>
{
    public Transform GridRoot;

    private bool btnStorageVisible = true;
    private bool btnTowerVisible = true;
    private bool btnSawmillVisible = true;
    private bool btnCancelVisible = false;
    private bool btnDeleteVisible = false;
    private bool btnMoveVisible = false;
    private bool placementGridVisible;
    
    void Start()
    {
        ShowBuildGUI();
        HidePlacementGrid();
    }

    void OnGUI()
    {
        if (btnStorageVisible)
        {
            if (GUI.Button(new Rect(20, 40, 80, 40), "Storage"))
            {
                GameManager.Instance.SelectStructure(0);
                ShowPlacementGUI();
            }
        }

        if (btnTowerVisible)
        {
            if (GUI.Button(new Rect(20, 90, 80, 40), "Tower"))
            {
                GameManager.Instance.SelectStructure(1);
                ShowPlacementGUI();
            }
        }

        if (btnSawmillVisible)
        {
            if (GUI.Button(new Rect(20, 140, 80, 40), "Sawmill"))
            {
                GameManager.Instance.SelectStructure(2);
                ShowPlacementGUI();
            }
        }

        if (btnCancelVisible)
        {
            if (GUI.Button(new Rect(20, 40, 80, 40), "Cancel"))
            {
                ShowBuildGUI();
            }
        }

        if (btnDeleteVisible)
        {
            if (GUI.Button(new Rect(20, 90, 80, 40), "Delete"))
            {
                GameManager.Instance.DeleteStructure();
            }
        }

        if (btnMoveVisible)
        {
            if (GUI.Button(new Rect(20, 140, 80, 40), "Move"))
            {
                GameManager.Instance.MoveStructure();
                ShowPlacementGrid();
            }
        }
    }

    private void ShowPlacementGrid()
    {
        if (!placementGridVisible)
        {
            foreach (Transform square in GridRoot)
            {
                square.gameObject.renderer.enabled = true;
            }
            placementGridVisible = true;
        }
    }

    private void HidePlacementGrid()
    {
        if (placementGridVisible)
        {
            foreach (Transform square in GridRoot)
            {
                square.gameObject.renderer.enabled = false;
            }
            placementGridVisible = false;
        }
    }

    public void ShowBuildGUI()
    {
        HideAllButtons();
        HidePlacementGrid();

        btnStorageVisible = true;
        btnTowerVisible = true;
        btnSawmillVisible = true;
    }

    public void ShowStructureGUI()
    {
        HideAllButtons();
        HidePlacementGrid();

        btnCancelVisible = true;
        btnDeleteVisible = true;
        btnMoveVisible = true;
    }

    private void ShowPlacementGUI()
    {
        HideAllButtons();
        ShowPlacementGrid();

        btnCancelVisible = true;
    }

    private void HideAllButtons()
    {
        btnStorageVisible = false;
        btnTowerVisible = false;
        btnSawmillVisible = false;
        btnCancelVisible = false;
        btnDeleteVisible = false;
        btnMoveVisible = false;
    }
}
