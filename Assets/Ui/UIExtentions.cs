using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIExtentions 
{
    public static void Display(this VisualElement element, bool enabled)
    {
        if(element == null) return;
        element.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static bool IsDisplayFlex(this VisualElement element) =>
        element != null && element.style.display == DisplayStyle.Flex;
}
