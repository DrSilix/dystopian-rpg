using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMenu
{
    
    /// <summary>
    /// Initialize the menu using the provided uiDoc. Take in and handle any info passed
    /// </summary>
    /// <param name="uiDoc">The UIDocument which contains the expected VisualTreeAsset</param>
    /// <param name="passInfo">A generic object for passing info of unknown type</param>
    public void InitializeMenu(UIDocument uiDoc, object passInfo);

    public void RegisterCallbacks();
    
    public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    public event EventHandler<object> UnloadMenu;

    public void UnregisterCallbacks();

    public void SendMenuNewInfo(object info);

    public void Update();
}
