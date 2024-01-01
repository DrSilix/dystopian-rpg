using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class SafeArea : MonoBehaviour
{
    UIDocument uiDoc;
    IPanel panel;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.panel;

        var safeLeftTop = RuntimePanelUtils.ScreenToPanel(
            panel,
            new Vector2(Screen.safeArea.xMin, Screen.height - Screen.safeArea.yMax)
        );
        var safeRightBottom = RuntimePanelUtils.ScreenToPanel(
            panel,
            new Vector2(Screen.width - Screen.safeArea.xMax, Screen.safeArea.yMin)
        );

        Debug.Log(safeLeftTop);
        Debug.Log(safeRightBottom) ;

        safeLeftTop.y = 136;


        uiDoc.rootVisualElement.style.paddingLeft = safeLeftTop.x;
        uiDoc.rootVisualElement.style.paddingRight = safeRightBottom.x;
        uiDoc.rootVisualElement.style.paddingTop = safeLeftTop.y;
        uiDoc.rootVisualElement.style.paddingBottom = safeRightBottom.y;
    }
}
