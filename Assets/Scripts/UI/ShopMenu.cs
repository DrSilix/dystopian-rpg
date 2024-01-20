using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopMenu : IMenu
{
    private string contactName;
    private Shop shop;
    private Inventory inventory;

    private VisualElement scrollableContentContainer;
    private VisualElement[] itemVisualElements;

    private Button confirmButton;
    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        contactName = passInfo as string;
        List<ContactSO> contacts = Storyteller.Instance.ContactSOs;
        ContactSO contact = contacts.Find(x => x.displayName == contactName);
        shop = new Shop(contact.shopInventorySO);

        VisualElement rootElem = uiDoc.rootVisualElement;

        confirmButton = rootElem.Q("confirm") as Button;

        scrollableContentContainer = rootElem.Q("unity-content-container");

        scrollableContentContainer.Clear();

        inventory = shop.Inventory;
        itemVisualElements = new VisualElement[shop.Inventory.Size];
        var i = 0;
        foreach (InventoryItem inventoryItem in shop.Inventory.GetAllItems())
        {
            VisualElement[] itemElement = GetBlankItemTemplate();
            itemElement[0].tooltip = inventoryItem.item.DisplayName;
            if (inventoryItem.item.Sprite != null) itemElement[1].style.backgroundImage = new StyleBackground(inventoryItem.item.Sprite);
            ((Label)itemElement[2]).text = inventoryItem.item.DisplayName;
            ((Label)itemElement[3]).text = $"${inventoryItem.item.Cost}";
            ((Label)itemElement[4]).text = inventoryItem.item.ToInventoryString();

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
        var price = new Label();
        var info = new Label();
        name.name = "name";
        price.name = "price";
        info.name = "info";
        infoTitleContainer.Add(name);
        infoTitleContainer.Add(price);
        infoContainer.Add(info);

        root.AddToClassList("item-container");
        itemIcon.AddToClassList("icon");
        infoContainer.AddToClassList("info-container");
        name.AddToClassList("name");
        price.AddToClassList("price");

        return new VisualElement[] { root, itemIcon, name, price, info };
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
                Debug.Log(target.tooltip);
                break;
            case "confirm":
                CallUnloadMenu(contactName);
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
