using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopMenu : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private string contactName;
    private Shop shop;
    private Inventory inventory;
    private Inventory crewInventory;

    private VisualElement scrollableContentContainer;
    private List<VisualElement> itemVisualElements;

    private Label contactNameElem, shopInfo1, shopInfo2;

    private VisualElement tempStoreElement;

    private Button confirmButton;
    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        contactName = passInfo as string;
        List<ContactSO> contacts = Storyteller.Instance.ContactSOs;
        ContactSO contact = contacts.Find(x => x.displayName == contactName);
        shop = new Shop(contact.shopInventorySO);

        VisualElement rootElem = uiDoc.rootVisualElement;

        confirmButton = rootElem.Q("confirm") as Button;
        contactNameElem = rootElem.Q("contact-name") as Label;
        shopInfo1 = rootElem.Q("shop-info-1") as Label;
        shopInfo2 = rootElem.Q("shop-info-2") as Label;

        scrollableContentContainer = rootElem.Q("unity-content-container");

        scrollableContentContainer.Clear();

        crewInventory = Storyteller.Instance.Crew.CrewInventory;

        inventory = shop.Inventory;
        inventory.Cash = contact.cashOnHand;
        itemVisualElements = new List<VisualElement>(shop.Inventory.Size);
        var i = 0;
        foreach (InventoryItem inventoryItem in shop.Inventory.GetAllItems())
        {
            VisualElement[] itemElement = GetBlankItemTemplate();
            itemElement[0].tooltip = inventoryItem.Id.ToString();
            if (inventoryItem.Item.Sprite != null) itemElement[1].style.backgroundImage = new StyleBackground(inventoryItem.Item.Sprite);
            ((Label)itemElement[2]).text = inventoryItem.Item.DisplayName;
            ((Label)itemElement[3]).text = $"${inventoryItem.Item.Cost}";
            ((Label)itemElement[4]).text = inventoryItem.Item.ToInventoryString();

            scrollableContentContainer.Add(itemElement[0]);
            itemVisualElements.Add(itemElement[0]);
            i++;
        }
        UpdateTopBarInfo();
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

    public void UpdateTopBarInfo()
    {
        contactNameElem.text = contactName;
        shopInfo1.text = $"Contact Cash\n${inventory.Cash}";
        shopInfo2.text = $"Crew Cash\n${crewInventory.Cash}";
    }

    public void RegisterCallbacks()
    {
        foreach (VisualElement element in itemVisualElements)
        {
            element.RegisterCallback<ClickEvent>(OnClick);
        }
        confirmButton.RegisterCallback<ClickEvent>(OnClick);
    }

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
                InventoryItem clickedItem = inventory.GetItemByInventoryItemId(int.Parse(target.tooltip));
                if (clickedItem.Item.Cost > crewInventory.Cash) break;
                CallLoadMenu("PurchaseConfirmOptions", true, clickedItem);
                tempStoreElement = target;
                break;
            case "confirm":
                CallUnloadMenu(contactName);
                break;
        }
    }
    public void SendMenuNewInfo(object info) {
        switch (info)
        {
            case InventoryItem e:
                shop.PurchaseItem(e, e.Item.Cost);
                tempStoreElement.parent.Remove(tempStoreElement);
                itemVisualElements.Remove(tempStoreElement);
                UpdateTopBarInfo();
                break;
        }
    }

    public void Update() { }
}
