using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMenu
{
    
    public void InitializeMenu();

    public void OnClick(ClickEvent e);

    public event EventHandler LoadMenu;
    public void CallLoadMenu(string menuName, int childNum);

    public string GetMenuName();
    public int GetChildNum();

    public void UnregisterCallbacks();
}
