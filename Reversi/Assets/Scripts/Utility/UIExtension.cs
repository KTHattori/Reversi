using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public static class UIExtension
{
    public static bool TryBindAction(this Button button,UnityAction action)
    {
        if(button != null) { button.onClick.AddListener(action); return true; }
        else { Debug.LogError("No button found."); return false; }
    }
}
