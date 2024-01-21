using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrewOptions : IMenu
{
    private Button inventoryButton;
    private Button returnButton;

    private object passInfo;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        this.passInfo = passInfo;

        VisualElement rootElem = uiDoc.rootVisualElement;

        returnButton = rootElem.Q("return") as Button;

        inventoryButton = rootElem.Q("inventory") as Button;
    }

    public void RegisterCallbacks()
    {
        returnButton.RegisterCallback<ClickEvent>(OnClick);
        inventoryButton.RegisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "inventory":
                CallLoadMenu("CrewInventoryMenu", true, passInfo);
                break;
            case "return":
                CallUnloadMenu(null);
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

    public void UnregisterCallbacks()
    {
        returnButton.UnregisterCallback<ClickEvent>(OnClick);
        inventoryButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    public void Update() { }
    public void SendMenuNewInfo(object info) { }
}