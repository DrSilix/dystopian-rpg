using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GearMenu : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private int crewMemberId;
    private CrewMemberController crewMember;
    private Weapon equippedWeapon;

    private Button doneButton;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        crewMemberId = 0;
        if (passInfo.GetType() == typeof(int)) crewMemberId = (int)passInfo;

        crewMember = Storyteller.Instance.Crew.GetCrewMember(crewMemberId);
        equippedWeapon = crewMember.EquippedItems.EquippedWeapon;

        VisualElement rootElem = uiDoc.rootVisualElement;

        populateWeaponData(rootElem.Q("weapon"));

        doneButton = rootElem.Q("done") as Button;
    }

    private void populateWeaponData(VisualElement rootElem)
    {
        ((Label)rootElem.Q("weapon-name")).text = equippedWeapon.DisplayName;

        VisualElement dataColumn1 = rootElem.Q("col1");

        Label[] descColumn = dataColumn1.Q("desc").Children().Cast<Label>().ToArray();
        Label[] valueColumn = dataColumn1.Q("value").Children().Cast<Label>().ToArray();
        descColumn[0].text = "Weapon Type"; valueColumn[0].text = equippedWeapon.WeaponType.ToString();
        descColumn[1].text = "Manufacturer"; valueColumn[1].text = equippedWeapon.Manufacturer;
        descColumn[2].text = "Accuracy"; valueColumn[2].text = equippedWeapon.Accuracy.ToString();
        descColumn[3].text = "Range"; valueColumn[3].text = equippedWeapon.Range.ToString();
        descColumn[4].text = "Damage"; valueColumn[4].text = equippedWeapon.Damage.ToString();
        descColumn[5].text = "Armor Piercing"; valueColumn[5].text = equippedWeapon.ArmorPiercing.ToString();
        descColumn[6].text = "Firing Mode"; valueColumn[6].text = equippedWeapon.FiringMode.ToString();
        descColumn[7].text = "Recoil"; valueColumn[7].text = equippedWeapon.Recoil.ToString();

        VisualElement dataColumn2 = rootElem.Q("col2");

        descColumn = dataColumn2.Q("desc").Children().Cast<Label>().ToArray();
        valueColumn = dataColumn2.Q("value").Children().Cast<Label>().ToArray();
        descColumn[0].text = "Ammo Capacity"; valueColumn[0].text = equippedWeapon.AmmoCapacity.ToString();
        descColumn[1].text = "Reload Time"; valueColumn[1].text = equippedWeapon.ReloadTime.ToString();
        descColumn[2].text = "Cost"; valueColumn[2].text = equippedWeapon.Cost.ToString();
        descColumn[3].text = "Availability"; valueColumn[3].text = equippedWeapon.Availability.ToString();
        //descColumn[4].text = "Illegal"; valueColumn[4].text = (equippedWeapon.Illegality > 0).ToString();
        descColumn[5].text = "Upper Attachment"; valueColumn[5].text = equippedWeapon.HasUpperAttachPoint.ToString();
        descColumn[6].text = "Lower Attachment"; valueColumn[6].text = equippedWeapon.HasLowerAttachPoint.ToString();
        descColumn[7].text = "Barrel Attachment"; valueColumn[7].text = equippedWeapon.HasBarrelAttachPoint.ToString();

        rootElem.Q("icon").style.backgroundImage = new StyleBackground(equippedWeapon.Sprite);
        ((Label)rootElem.Q("long-desc")).text = equippedWeapon.LongDescription;
    }

    public void RegisterCallbacks()
    {
        doneButton.RegisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "done":
                CallUnloadMenu(crewMemberId);
                break;
        }
    }

    public void SendMenuNewInfo(object info) { }

    public void UnregisterCallbacks()
    {
        doneButton.UnregisterCallback<ClickEvent>(OnClick);
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

    public void Update() { }
}
