using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;

public class CrewAttributesMenu : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private static readonly int MAX_ATTRIBUTE_VALUE = 6;

    private Button doneButton;
    private int crewMemberId;

    private CrewMemberController crewMember;
    private Attributes attributes;
    private Label[] attributeLabels = new Label[8];
    private Button[] attributeButtons = new Button[16];
    private Label unspentLabel;

    private int unspentPoints;
    private int startingPoints;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        crewMemberId = 0;
        if (passInfo.GetType() == typeof(int)) crewMemberId = (int)passInfo;

        crewMember = Storyteller.Instance.Crew.GetCrewMember(crewMemberId);
        attributes = crewMember.attributes;

        VisualElement rootElem = uiDoc.rootVisualElement;

        doneButton = rootElem.Q("done") as Button;

        unspentLabel = rootElem.Q("unspent-points") as Label;

        for (int i = 0; i < 8; i++)
        {
            VisualElement attributeRootElem = rootElem.Q(((Attribute)i).ToString());
            attributeLabels[i] = attributeRootElem.Q("value") as Label;
            attributeLabels[i].text = crewMember.GetAttribute((Attribute)i).ToString();
            attributeButtons[i * 2] = attributeRootElem.Q("button-plus") as Button;
            attributeButtons[(i * 2) + 1] = attributeRootElem.Q("button-minus") as Button;
        }

        startingPoints = 9;
        unspentPoints = 9;
    }

    public void RegisterCallbacks()
    {
        doneButton.RegisterCallback<ClickEvent>(OnClick);
        for (int i = 0; i < attributeButtons.Length; i++)
        {
            attributeButtons[i].RegisterCallback<ClickEvent>(OnClick);
        }
    }

    /*private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "button-plus":
                ModifyValue((VisualElement)e.currentTarget);
                break;
            case "button-minus":
                ModifyValue((VisualElement)e.currentTarget);
                break;
            case "done":
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
            case "button-plus":
                ModifyValue((VisualElement)e.currentTarget);
                break;
            case "button-minus":
                ModifyValue((VisualElement)e.currentTarget);
                break;
            case "done":
                CallUnloadMenu(crewMemberId);
                break;
        }
    }

    private void ModifyValue(VisualElement element)
    {
        if (element.name == "button-plus" && unspentPoints <= 0) return;
        if (element.name == "button-minus" && unspentPoints >= startingPoints) return;

        string attributeName = element.parent.parent.name;
        int unmodifiedValue = attributes.Get(attributeName);
        Label valueLabel = element.parent.Q("value") as Label;
        int value = int.Parse(valueLabel.text);

        if (element.name == "button-plus" && value >= MAX_ATTRIBUTE_VALUE) return;
        if (element.name == "button-minus" && value <= unmodifiedValue) return;

        if (element.name == "button-plus")
        {
            valueLabel.text = (++value).ToString();
            unspentPoints--;
        }
        if (element.name == "button-minus")
        {
            valueLabel.text = (--value).ToString();
            unspentPoints++;
        }
        unspentLabel.text = unspentPoints.ToString();
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
        doneButton.UnregisterCallback<ClickEvent>(OnClick);
        for (int i = 0; i < attributeButtons.Length; i++)
        {
            attributeButtons[i].UnregisterCallback<ClickEvent>(OnClick);
        }
    }

    public void Update() { }
    public void SendMenuNewInfo(object info) { }
}
