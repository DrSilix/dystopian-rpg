using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ItemSlot = EquippedItems.ItemSlot;

public class GearSlotOptions : MenuOptionsBase, IMenu
{
    CrewMemberController crewMember;
    IInventoryItem item;
    public override void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        base.InitializeMenu(uiDoc, passInfo);

        if (passInfo.GetType() != typeof((CrewMemberController, InventoryItem))) return;
        (CrewMemberController, InventoryItem) info = ((CrewMemberController, InventoryItem))passInfo;
        crewMember = info.Item1;
        item = info.Item2.Item;

        if (item != null)
        {
            foreach (VisualElement button in Buttons) button.style.display = DisplayStyle.None;
            Buttons[Buttons.Count - 1].style.display = DisplayStyle.Flex;
            switch (item)
            {
                case Weapon e:
                    Buttons[0].style.display = DisplayStyle.Flex;
                    Buttons[1].style.display = DisplayStyle.Flex;
                    break;
                case Armor e:
                    Buttons[2].style.display = DisplayStyle.Flex;
                    Buttons[3].style.display = DisplayStyle.Flex;
                    break;
            }
        }


        Dictionary<ItemSlot, InventoryItem> equipment = crewMember.EquippedItems.GetEquipment();
        InventoryItem mainWeapon = equipment[ItemSlot.MainWeapon];
        if (mainWeapon != null) ((Button)Buttons[0]).text = $"Main Weapon\n<size=30>{mainWeapon.Item.DisplayName}</size>";
        InventoryItem mainArmor = equipment[ItemSlot.MainArmor];
        if (mainArmor != null) ((Button)Buttons[2]).text = $"Primary Armor\n<size=30>{mainArmor.Item.DisplayName}</size>";
    }

    public override void OnClick(ClickEvent e)
    {
        string targetName = ((VisualElement)e.currentTarget).name;
        Debug.Log(targetName);

        switch (targetName)
        {
            case "main-weapon":
                CallUnloadMenu(ItemSlot.MainWeapon, false);
                break;
            case "offhand-weapon":
                break;
            case "main-armor":
                CallUnloadMenu(ItemSlot.MainArmor, false);
                break;
            case "sub-armor":
                break;
            case "item-one":
                break;
            case "item-two":
                break;
            case "return":
                CallUnloadMenu(null);
                break;
        }
    }
}
