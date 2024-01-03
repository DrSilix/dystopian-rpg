using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CrewMenu : IMenu
{
    private VisualElement crewMember1, crewMember2, crewMember3;
    private Button doneButton;

    /*public void OnEnable()
    {
        InitializeMenu();
    }*/

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        doneButton = rootElem.Q("done-button") as Button;

        crewMember1 = rootElem.Q("crew-member-1");
        crewMember2 = rootElem.Q("crew-member-2");
        crewMember3 = rootElem.Q("crew-member-3");

        UpdateCrewMemberInfo(crewMember1, 1);
        UpdateCrewMemberInfo(crewMember2, 2);
        UpdateCrewMemberInfo(crewMember3, 3);
    }

    public void RegisterCallbacks()
    {
        doneButton.RegisterCallback<ClickEvent>(OnClick);
        crewMember1.RegisterCallback<ClickEvent>(OnClick);
        crewMember2.RegisterCallback<ClickEvent>(OnClick);
        crewMember3.RegisterCallback<ClickEvent>(OnClick);
    }

    private void UpdateCrewMemberInfo(VisualElement crewMember, int crewId)
    {
        CrewMemberController memberController = Storyteller.Instance.Crew.GetCrewMember(crewId);

        Label title = crewMember.Q("title-bar") as Label;
        title.text = memberController.alias + " - Lvl 4 - ";

        VisualElement icon = crewMember.Q("icon");
        Label[] attributes = new Label[8];
        Label[] stats = new Label[4];
        
        for (int i = 0; i < 8; i++)
        {
            attributes[i] = crewMember.Q(((Attribute)i).ToString() + "-value") as Label;
            attributes[i].text = memberController.GetAttribute((Attribute)i).ToString();
        }
    }

    /*private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "crew-member-1":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "crew-member-2":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "crew-member-3":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "done-button":
                UnregisterCallbacks();
                uiDoc.enabled = false;
                break;
        }
    }*/

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "crew-member-1":
                CallLoadMenu("CrewMemberOptions", true, 1);
                break;
            case "crew-member-2":
                CallLoadMenu("CrewMemberOptions", true, 2);
                break;
            case "crew-member-3":
                CallLoadMenu("CrewMemberOptions", true, 3);
                break;
            case "done-button":
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
        doneButton.UnregisterCallback<ClickEvent>(OnClick);
        crewMember1.UnregisterCallback<ClickEvent>(OnClick);
        crewMember2.UnregisterCallback<ClickEvent>(OnClick);
        crewMember3.UnregisterCallback<ClickEvent>(OnClick);
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
