using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrewInventoryMenu : IMenu
{
    private CrewController crewController;
    private Inventory inventory;

    private VisualElement scrollableContentContainer;
    private VisualElement[] itemVisualElements;

    private Button confirmButton;
    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        crewController = Storyteller.Instance.Crew;
        VisualElement rootElem = uiDoc.rootVisualElement;

        confirmButton = rootElem.Q("confirm") as Button;

        scrollableContentContainer = rootElem.Q("unity-content-container");

        scrollableContentContainer.Clear();

        inventory = crewController.CrewInventory;
        itemVisualElements = new VisualElement[inventory.Size];
        var i = 0;
        foreach (InventoryItem inventoryItem in inventory.GetAllItems())
        {
            VisualElement[] itemElement = GetBlankItemTemplate();
            itemElement[0].tooltip = inventoryItem.Id.ToString();
            if (inventoryItem.Item.Sprite != null) itemElement[1].style.backgroundImage = new StyleBackground(inventoryItem.Item.Sprite);
            ((Label)itemElement[2]).text = inventoryItem.Item.DisplayName;
            if (inventoryItem.CurrentlyEquippedBy != null) ((Label)itemElement[3]).text = $"Equipped by: {inventoryItem.CurrentlyEquippedBy.alias}";
            ((Label)itemElement[4]).text = inventoryItem.Item.ToInventoryString();

            scrollableContentContainer.Add(itemElement[0]);
            itemVisualElements[i] = itemElement[0];
            i++;
        }
    }

    public VisualElement[] GetBlankItemTemplate()
    {
        var root = new VisualElement();
        root.name = "item-container";
        var itemIcon = new VisualElement();
        itemIcon.name = "icon";
        root.Add(itemIcon);

        var infoContainer = new VisualElement();
        root.Add(infoContainer);

        var infoTitleContainer = new VisualElement();
        infoTitleContainer.style.flexDirection = FlexDirection.Row;
        infoContainer.Add(infoTitleContainer);

        var name = new Label();
        var equipped = new Label();
        var info = new Label();
        name.name = "name";
        equipped.name = "equipped";
        info.name = "info";
        infoTitleContainer.Add(name);
        infoTitleContainer.Add(equipped);
        infoContainer.Add(info);

        root.AddToClassList("item-container");
        itemIcon.AddToClassList("icon");
        infoContainer.AddToClassList("info-container");
        name.AddToClassList("name");
        equipped.AddToClassList("equipped");

        return new VisualElement[] { root, itemIcon, name, equipped, info };
    }

    public void RegisterCallbacks()
    {
        foreach (VisualElement element in itemVisualElements)
        {
            element.RegisterCallback<ClickEvent>(OnClick);
        }
        confirmButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void SendMenuNewInfo(object info) { }

    public void UnregisterCallbacks()
    {
        foreach (VisualElement element in itemVisualElements)
        {
            element.UnregisterCallback<ClickEvent>(OnClick);
        }
        confirmButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        VisualElement target = ((VisualElement)e.currentTarget);
        Debug.Log(target.name);
        switch (target.name)
        {
            case "item-container":
                CallLoadMenu("InventoryItemOptions", true, (target.tooltip, inventory));
                break;
            case "confirm":
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

    public void Update() { }
}
