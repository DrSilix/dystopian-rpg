using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static EquippedItems;

public class PurchaseConfirmOptions : MenuOptionsBase, IMenu
{
    public override void OnClick(ClickEvent e)
    {
        string targetName = ((VisualElement)e.currentTarget).name;
        Debug.Log(targetName);

        switch (targetName)
        {
            case "confirm":
                CallUnloadMenu(PassInfo, false);
                break;
            case "return":
                CallUnloadMenu(null, false);
                break;
        }
    }
}
