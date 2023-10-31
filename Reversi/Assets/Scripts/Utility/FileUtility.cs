using UnityEngine;

/// <summary>
/// ファイル位置取得などの機能追加用
/// </summary>
public static class FileUtility
{
    // ファイルパス取得・変換系
    // 参考：https://compota-soft.work/archives/2480

    /// <summary>
    /// この関数をコールしたスクリプトのファイルパス(OS の絶対ファイルパス)を取得する関数。
    /// </summary>
    /// <param name="filePath">この関数をコールしたスクリプトへのファイルパス（自動的に設定される）</param>
    /// <returns>ファイルパス</returns>
    /// <see cref="roslyn - C# for scripting (csx) location of script file - Stack Overflow - https://stackoverflow.com/questions/46728845/c-sharp-for-scripting-csx-location-of-script-file>"/>
    public static string GetCallerFilePath([System.Runtime.CompilerServices.CallerFilePath] string filePath = null)
    {
        return filePath;
    }

    /// <summary>
    /// ファイルパスをアセットパスへ変換する関数。
    /// <see cref="AssetDatabase-GetAssetPath - Unity スクリプトリファレンス - https://docs.unity3d.com/ja/2018.4/ScriptReference/AssetDatabase.GetAssetPath.html>"/>
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <returns>アセットパス</returns>
    public static string ConvertToAssetPath(string filePath)
    {
        return filePath.Replace(System.IO.Path.DirectorySeparatorChar, '/').Replace(Application.dataPath, "Assets");
    }

    /// <summary>
    /// アセットパスをファイルパスへ変換する関数。
    /// </summary>
    /// <param name="filePath">アセットパス</param>
    /// <returns>ファイルパス</returns>
    public static string ConvertToFilePath(string assetPath)
    {
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return assetPath.Replace("Assets", Application.dataPath).Replace('/', System.IO.Path.DirectorySeparatorChar);
    #else
        return assetPath.Replace("Assets", Application.dataPath);
    #endif
    }

    /// <summary>
    /// この関数をコールしたスクリプトのファイルパスをアセットパスに変換して取得する関数。
    /// </summary>
    /// <param name="filePath">コールしたスクリプトへのファイルパスが自動で代入されます</param>
    /// <returns>コールしたスクリプトへのアセットパス</returns>
    public static string GetCallerAssetPath([System.Runtime.CompilerServices.CallerFilePath] string filePath = null)
    {
        return filePath.Replace(System.IO.Path.DirectorySeparatorChar, '/').Replace(Application.dataPath, "Assets");
    }
}
