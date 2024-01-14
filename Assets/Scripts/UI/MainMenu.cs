using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MainMenu : IMenu
{
    private VisualElement startButton;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        startButton = rootElem.Q("game-start");
    }

    public void RegisterCallbacks()
    {
        startButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void SendMenuNewInfo(object info) { }

    public void UnregisterCallbacks()
    {
        startButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "game-start":
                CallLoadMenu("SafehouseMenu", false, null);
                break;
        }
    }

    public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    private void CallLoadMenu(string menuName, bool isChild, object passInfo)
    {
        LoadMenu.Invoke(this, (menuName, isChild, passInfo));
    }

    public event EventHandler<object> UnloadMenu;

    private void CallUnloadMenu(object passInfo)
    {
        UnloadMenu.Invoke(this, passInfo);
    }

    public void Update() { }
}
