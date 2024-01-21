using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CrewMenu : DefaultLoadUnloadMenuBehaviour, IMenu
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

        UpdateCrewMemberInfo(crewMember1, 0);
        UpdateCrewMemberInfo(crewMember2, 1);
        UpdateCrewMemberInfo(crewMember3, 2);
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

        Weapon equippedWeapon = memberController.EquippedItems.EquippedWeapon;

        crewMember.Q("weapon-icon").style.backgroundImage = new StyleBackground(equippedWeapon.Sprite);
        ((Label)crewMember.Q("weapon-name")).text = "Name: " + equippedWeapon.DisplayName;
        ((Label)crewMember.Q("weapon-type")).text = equippedWeapon.WeaponType.ToString();
        ((Label)crewMember.Q("weapon-stats1")).text = equippedWeapon.Accuracy.ToString() + "/" +
                                                      equippedWeapon.Damage.ToString() + "/" +
                                                      equippedWeapon.ArmorPiercing.ToString() + "/" +
                                                      equippedWeapon.Recoil.ToString();
        ((Label)crewMember.Q("weapon-stats2")).text = (equippedWeapon.CurrentAmmoCount + equippedWeapon.AmmoHeld).ToString() + "/" +
                                                        equippedWeapon.AmmoCapacity.ToString() + " - " +
                                                        equippedWeapon.AmmunitionSO.displayName;

        Armor equippedArmor = memberController.EquippedItems.EquippedArmor;

        crewMember.Q("armor-icon").style.backgroundImage = new StyleBackground(equippedArmor.Sprite);
        ((Label)crewMember.Q("armor-name")).text = "Name: " + equippedArmor.DisplayName;
        ((Label)crewMember.Q("armor-type").parent.Children().First()).text = "Manf:";
        ((Label)crewMember.Q("armor-type")).text = equippedArmor.Manufacturer;
        ((Label)crewMember.Q("armor-stats1")).text = equippedArmor.ArmorRating.ToString();
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
                CallLoadMenu("CrewMemberOptions", true, 0);
                break;
            case "crew-member-2":
                CallLoadMenu("CrewMemberOptions", true, 1);
                break;
            case "crew-member-3":
                CallLoadMenu("CrewMemberOptions", true, 2);
                break;
            case "done-button":
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
        doneButton.UnregisterCallback<ClickEvent>(OnClick);
        crewMember1.UnregisterCallback<ClickEvent>(OnClick);
        crewMember2.UnregisterCallback<ClickEvent>(OnClick);
        crewMember3.UnregisterCallback<ClickEvent>(OnClick);
    }

    public void Update() { }
    public void SendMenuNewInfo(object info) { }
}
