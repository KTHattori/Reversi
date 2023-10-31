using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AssetImporters;

[ScriptedImporter(version:1,ext:ReversiBookAsset.Extension)]
public class ReversiBookImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
       // インポートされるファイルのパスを取得
        string text = File.ReadAllText(ctx.assetPath);

        // TextAssetを作成してテキストを格納
        TextAsset textAsset = new TextAsset(text);

        // アセットをコンテキストに追加
        ctx.AddObjectToAsset("main", textAsset);
        ctx.SetMainObject(textAsset);
    }
}
#endif
