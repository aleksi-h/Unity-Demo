﻿using UnityEngine;
using System.Collections;

/*
 * A singleton GUI manager that persists between scenes
 * 
 */
public class GUIManager : Singleton<GUIManager>
{
    public Transform GridRoot;

    private bool btnStorageVisible = true;
    private bool btnSawmillVisible = true;
    private bool btnHutVisible = true;
    private bool btnFieldVisible = true;
    private bool btnCancelVisible = false;
    private bool btnDeleteVisible = false;
    private bool btnMoveVisible = false;
    private bool btnConfirmVisible = false;
    private bool placementGridVisible;
    
    void Start()
    {
        HideAllGUIElements();
        ShowDefaultGUI();
    }

    void OnGUI()
    {
        if (btnStorageVisible)
        {
            if (GUI.Button(new Rect(20, 40, 80, 40), "Storage"))
            {
                BuildingManager.instance.BuildStructure(BuildingManager.instance.storage);
            }
        }

        if (btnSawmillVisible)
        {
            if (GUI.Button(new Rect(20, 90, 80, 40), "Sawmill"))
            {
                BuildingManager.instance.BuildStructure(BuildingManager.instance.sawmill);
            }
        }

        if (btnHutVisible)
        {
            if (GUI.Button(new Rect(20, 140, 80, 40), "Hut"))
            {
                BuildingManager.instance.BuildStructure(BuildingManager.instance.hut);
            }
        }

        if (btnFieldVisible)
        {
            if (GUI.Button(new Rect(20, 190, 80, 40), "Field"))
            {
                BuildingManager.instance.BuildStructure(BuildingManager.instance.field);
            }
        }

        if (btnCancelVisible)
        {
            if (GUI.Button(new Rect(20, 40, 80, 40), "Cancel"))
            {
                ShowDefaultGUI();
            }
        }

        if (btnDeleteVisible)
        {
            if (GUI.Button(new Rect(20, 90, 80, 40), "Delete"))
            {
                BuildingManager.Instance.DeleteStructure();
            }
        }

        if (btnMoveVisible)
        {
            if (GUI.Button(new Rect(20, 140, 80, 40), "Move"))
            {
                BuildingManager.Instance.MoveStructure();
            }
        }

        if (btnConfirmVisible)
        {
            if (GUI.Button(new Rect(20, 40, 80, 40), "Confirm"))
            {
                BuildingManager.Instance.ConfirmBuild();
            }
        }
    }

    
    public void ShowDefaultGUI()
    {
        HideAllGUIElements();

        btnStorageVisible = true;
        btnSawmillVisible = true;
        btnHutVisible = true;
        btnFieldVisible = true;
    }

    public void ShowStructureGUI()
    {
        HideAllGUIElements();

        btnCancelVisible = true;
        btnDeleteVisible = true;
        btnMoveVisible = true;
    }

    public void ShowPlacementGUI()
    {
        HideAllGUIElements();

        //btnCancelVisible = true;
        btnConfirmVisible = true;
        Grid.Instance.HighLightFreeNodes();
    }

    private void HideAllGUIElements()
    {
        Grid.Instance.HideHighlight();
        btnStorageVisible = false;
        btnSawmillVisible = false;
        btnHutVisible = false;
        btnFieldVisible = false;
        btnCancelVisible = false;
        btnDeleteVisible = false;
        btnMoveVisible = false; 
        btnConfirmVisible = false;
    }
}
