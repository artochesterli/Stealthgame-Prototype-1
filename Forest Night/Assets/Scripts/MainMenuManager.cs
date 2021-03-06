﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Rewired;

public class MainMenuManager : MonoBehaviour
{
    public GameObject ContinueButton;

    private bool ContinueAvaliable;
    private bool Active;

    private const string FolderName = "PlayerData";
    private const string FileName = "PlayerData";
    private const string Extension = ".dat";
    private const float StartY = 0;
    private const float Interval = 100;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        
        EventManager.instance.AddHandler<EnterMenu>(OnEnterMenu);
        EventManager.instance.AddHandler<ExitMenu>(OnExitMenu);

    }

    private void OnDestroy()
    {
        EventManager.instance.RemoveHandler<EnterMenu>(OnEnterMenu);
        EventManager.instance.RemoveHandler<ExitMenu>(OnExitMenu);
    }

    private bool HaveData()
    {
        string FolderPath = Path.Combine(Application.dataPath, FolderName);
        string DataPath = Path.Combine(FolderPath, FileName + Extension);
        if (!File.Exists(DataPath))
        {
            return false;
        }

        if (SaveDataManager.data.CurrentSaveSlot < 0)
        {
            return false;
        }

        for(int i = 0; i < SaveDataManager.data.Progress.Count; i++)
        {
            if (SaveDataManager.data.Progress[SaveDataManager.data.CurrentSaveSlot]>0 && !SaveDataManager.data.Finish[SaveDataManager.data.CurrentSaveSlot])
            {
                return true;
            }
        }

        return false;
    }

    private void OnEnterMenu(EnterMenu E)
    {
        if (E.Menu == gameObject)
        {
            MenuGroupManager.CurrentActivatedMenu = gameObject;
            GetComponent<ButtonSelection>().enabled = true;
            SetMenu();
        }
    }

    private void OnExitMenu(ExitMenu E)
    {
        if (E.Menu == gameObject)
        {
            GetComponent<ButtonSelection>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void SetMenu()
    {
        float PivotY;
        if (HaveData())
        {
            PivotY = StartY;
            if (GetComponent<ButtonSelection>().ButtonList[0]!=ContinueButton)
            {
                GetComponent<ButtonSelection>().ButtonList.Insert(0, ContinueButton);
            }
        }
        else
        {
            PivotY = StartY - Interval / 2;
            ContinueButton.SetActive(false);
            if(GetComponent<ButtonSelection>().ButtonList[0] == ContinueButton)
            {
                GetComponent<ButtonSelection>().ButtonList.RemoveAt(0);
            }
        }

        for(int i = 0; i < GetComponent<ButtonSelection>().ButtonList.Count; i++)
        {
            GetComponent<ButtonSelection>().ButtonList[i].GetComponent<RectTransform>().anchoredPosition= new Vector2(0, PivotY - i * Interval);
            GetComponent<ButtonSelection>().ButtonList[i].SetActive(true);
        }
        
    }

}
