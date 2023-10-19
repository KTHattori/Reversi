using System;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// リバーシ定石ファイルをTextAssetのように扱うためのクラス
/// </summary>
[Serializable]
public class ReversiBookAsset
{
    public const string Extension = ".rbook";

    [SerializeField] private string path;

    [SerializeField] private string textString;

    [SerializeField] private string byteString;

    public string text => textString;

    public byte[] bytes => Encoding.ASCII.GetBytes(byteString);

    public ReversiBookAsset(string content)
    {
        textString = content;
    }

    public static implicit operator TextAsset(ReversiBookAsset bookAsset)
    {
        return new TextAsset(bookAsset.textString);
    }

    public static implicit operator ReversiBookAsset(TextAsset textAsset)
    {
        return new ReversiBookAsset(textAsset.text);
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReversiBookAsset))]
public class ReversiBookInspectorEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var path = property.FindPropertyRelative("path").stringValue;
        var loaded = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
        var field = EditorGUI.ObjectField(position, label, loaded, typeof(TextAsset), false);
        var loadPath = AssetDatabase.GetAssetPath(field);
        var fileExtension = Path.GetExtension(loadPath);
        if (field == null || fileExtension != ReversiBookAsset.Extension)
        {
            property.SetString("path", "");
            property.SetString("textString", "");
            property.SetString("byteString", "");
        }
        else
        {
            var pathProperty = property.FindPropertyRelative("path");
            property.SetString("path", loadPath.Substring(loadPath.IndexOf("Assets", StringComparison.Ordinal)));
            property.SetString("textString", File.ReadAllText(pathProperty.stringValue));
            property.SetString("byteString", Encoding.ASCII.GetString(File.ReadAllBytes(pathProperty.stringValue)));
        }
    }
}
#endif