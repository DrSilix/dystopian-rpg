using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrewMemberOptions : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private Button attributesButton;
    private Button gearButton;
    private Button returnButton;

    private int crewMemberId;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        crewMemberId = 0;
        if (passInfo.GetType() == typeof(int)) crewMemberId = (int)passInfo;
        
        VisualElement rootElem = uiDoc.rootVisualElement;

        returnButton = rootElem.Q("return") as Button;

        attributesButton = rootElem.Q("attributes") as Button;
        gearButton = rootElem.Q("gear") as Button;
    }

    public void RegisterCallbacks()
    {
        returnButton.RegisterCallback<ClickEvent>(OnClick);
        attributesButton.RegisterCallback<ClickEvent>(OnClick);
        gearButton.RegisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "attributes":
                CallLoadMenu("CrewAttributesMenu", true, crewMemberId);
                break;
            case "gear":
                CallLoadMenu("GearMenu", true, crewMemberId);
                break;
            case "return":
                CallUnloadMenu(null);
                break;
        }
    }

    /*public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    private void CallLoadMenu(string menuName, bool isChild, object passInfo)
    {
        LoadMenu.Invoke(this, (menuName, isChild, passInfo));
    }

    public event EventHandler<object> UnloadMenu;

    private void CallUnloadMenu(object passInfo)
    {
        UnloadMenu.Invoke(this, passInfo);
    }*/

    public void UnregisterCallbacks()
    {
        returnButton.UnregisterCallback<ClickEvent>(OnClick);
        attributesButton.UnregisterCallback<ClickEvent>(OnClick);
        gearButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    public void Update() { }
    public void SendMenuNewInfo(object info) { }
}
