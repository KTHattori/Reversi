#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
public class StartSceneConstrain
{
    static StartSceneConstrain()
    {
        // In Unity Editor, set Play Mode scene to LOGIN scene
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/TitleScene.unity");
    }
}
#endif

