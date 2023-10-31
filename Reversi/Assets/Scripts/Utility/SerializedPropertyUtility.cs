#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// シリアライズされたプロパティの拡張
/// </summary>
public static class SerializedPropertyUtility
{
    /// <summary>
    /// プロパティ
    /// </summary>
    /// <param name="property"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void SetString(this SerializedProperty property, string name, string value)
    {
        var pathProperty = property.FindPropertyRelative(name);
        pathProperty.stringValue = value;
    }
}
#endif