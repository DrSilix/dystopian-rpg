using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMenu
{
    
    public void InitializeMenu(UIDocument uiDoc, object passInfo);

    public void RegisterCallbacks();
    
    public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    public event EventHandler<object> UnloadMenu;

    public void UnregisterCallbacks();
}
