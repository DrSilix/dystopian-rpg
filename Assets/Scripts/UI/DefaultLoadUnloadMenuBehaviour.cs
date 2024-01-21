using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultLoadUnloadMenuBehaviour
{
    public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    public void CallLoadMenu(string menuName, bool isChild, object passInfo)
    {
        LoadMenu.Invoke(this, (menuName, isChild, passInfo));
    }

    public event EventHandler<(object passInfo, bool resetParentMenu)> UnloadMenu;

    public void CallUnloadMenu(object passInfo, bool resetParentMenu)
    {
        UnloadMenu.Invoke(this, (passInfo, resetParentMenu));
    }

    public void CallUnloadMenu(object passInfo)
    {
        UnloadMenu.Invoke(this, (passInfo, true));
    }
}
