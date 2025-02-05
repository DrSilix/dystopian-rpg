using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class InventoryItemOptions : MenuOptionsBase, IMenu
{
    private InventoryItem item;
    private CrewMemberController crewMember;
    public override void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        base.InitializeMenu(uiDoc, passInfo);

        if (passInfo?.GetType() != typeof(InventoryItem)) return;
        item = (InventoryItem)passInfo;

        if (item.CurrentlyEquippedBy != null)
        {
            Buttons[1].style.display = DisplayStyle.None;
            ((Button)Buttons[2]).text = $"Unequip\n<size=30>{item.CurrentlyEquippedBy.alias}</size>";
        }
        else
        {
            Buttons[2].style.display = DisplayStyle.None;
        }
    }

    public override void OnClick(ClickEvent e)
    {
        string targetName = ((VisualElement)e.currentTarget).name;
        Debug.Log(targetName);
        switch (targetName)
        {
            case "equip":
                CallLoadMenu("ChooseCrewMemberOptions", true, null);
                break;
            case "unequip":
                item.CurrentlyEquippedBy.EquippedItems.Unequip(item);
                CallUnloadMenu(null);
                break;
            case "return":
                CallUnloadMenu(null);
                return;
            default:
                CallLoadMenu(targetName, true, PassInfo);
                break;
        }
    }

    public override void SendMenuNewInfo(object info)
    {
        //Handle equipping
        switch (info)
        {
            case CrewMemberController e:
                crewMember = e;
                CallLoadMenu("GearSlotOptions", true, (crewMember, item));
                break;
            case EquippedItems.ItemSlot e:
                crewMember.EquippedItems.Equip(e, item);
                CallUnloadMenu(null);
                break;
        }
    }
}
