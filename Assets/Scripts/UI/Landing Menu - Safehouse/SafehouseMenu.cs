using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SafehouseMenu : IMenu
{
    public EventSystem eventSystem;
    public GameObject hideoutImage;

    public VisualElement[] crewMemberElement;

    public CrewController crewController;

    private VisualElement crewStatusButton;
    private Button planJobButton;
    private Button contactsButton;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        crewStatusButton = rootElem.Q("crewstatus");
        planJobButton = rootElem.Q("planjob") as Button;
        contactsButton = rootElem.Q("contacts") as Button;

        crewController = Storyteller.Instance.Crew;

        crewMemberElement = new VisualElement[]
        {
            rootElem.Q("crewmember1"),
            rootElem.Q("crewmember2"),
            rootElem.Q("crewmember3")
        };

        for (int i = 0; i < crewMemberElement.Length; i++)
        {
            FillInCrewMemberInfo(crewMemberElement[i], i);
        }
    }

    public void FillInCrewMemberInfo(VisualElement crewMemberElement, int crewMemberID)
    {
        if (crewMemberID + 1 <= crewController.CrewMembers.Count)
        {
            CrewMemberController crewMember = crewController.GetCrewMember(crewMemberID);
            crewMemberElement.Q("portrait").style.backgroundImage = new StyleBackground(crewMember.Icon);
            ((Label)crewMemberElement.Q("title")).text = $"{crewMember.alias} - Lvl 4 - Human - Enforcer";
            Label info1 = crewMemberElement.Q("info-column-1") as Label;
            info1.text = $"<b>Dmg:</b> {crewMember.DamageTaken}/{crewMember.MaxDamage}\n" +
                            $"<b>Init:</b> {crewMember.InitiativeDice}D6 + {crewMember.InitiativeModifier}\n" +
                            $"<b>State:</b> Aggressive\n" +
                            $"<b>Item:</b> HE Grenade";
            Label info2 = crewMemberElement.Q("info-column-2") as Label;
            Weapon weapon = crewMember.EquippedItems.EquippedWeapon;
            Armor armor = crewMember.EquippedItems.EquippedArmor;
            info2.text = "";
            if (weapon != null)
            {
                info2.text += $"<b>Weap:</b> {weapon.DisplayName}\n" +
                              $"Acc: {weapon.Accuracy}\tMode: {weapon.FiringMode}\n" +
                              $"Dmg: {weapon.Damage}\tAmmo: {weapon.CurrentAmmoCount}/{weapon.AmmoCapacity}\n";
            }
            if (armor != null)
            {
                info2.text += $"<b>Wear:</b> {armor.DisplayName}\n" +
                              $"Armor: {armor.ArmorRating}";
            }
        }
    }

    public void RegisterCallbacks()
    {
        crewStatusButton.RegisterCallback<ClickEvent>(OnClick);
        planJobButton.RegisterCallback<ClickEvent>(OnClick);
        contactsButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void UnregisterCallbacks()
    {
        crewStatusButton.UnregisterCallback<ClickEvent>(OnClick);
        planJobButton.UnregisterCallback<ClickEvent>(OnClick);
        contactsButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "crewstatus":
                CallLoadMenu("CrewOptions", true, null);
                break;
            case "planjob":
                CallLoadMenu("HeistHUD", false, null);
                Storyteller.Instance.StartHeist();
                break;
            case "contacts":
                CallLoadMenu("ContactsMenu", false, null);
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

    public void SendMenuNewInfo(object info) { }
}
