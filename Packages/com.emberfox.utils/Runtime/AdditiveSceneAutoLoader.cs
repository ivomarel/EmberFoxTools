using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class AdditiveSceneAutoLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;   // Drag a scene here (Editor-only)
#endif

    [SerializeField, HideInInspector] private string scenePath; // Saved for runtime + builds

    private void OnEnable() => TryLoad();

    private void OnValidate()
    {
        CacheScenePath();
    } 

    private void CacheScenePath()
    {
#if UNITY_EDITOR
        scenePath = sceneAsset ? AssetDatabase.GetAssetPath(sceneAsset) : "";
#endif
    }

    private void TryLoad()
    {
        if (string.IsNullOrEmpty(scenePath)) return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var alreadyOpen = EditorSceneManager.GetSceneByPath(scenePath).isLoaded;
            if (!alreadyOpen)
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            return;
        }
#endif

        // Runtime
        if (!IsLoaded(scenePath))
            SceneManager.LoadSceneAsync(GetSceneName(scenePath), LoadSceneMode.Additive);
    }

    private static bool IsLoaded(string path)
    {
        var name = GetSceneName(path);
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).name == name) return true;
        return false;
    }

    private static string GetSceneName(string path)
    {
        // "Assets/Scenes/MyScene.unity" -> "MyScene"
        var slash = path.LastIndexOf('/') + 1;
        var dot = path.LastIndexOf('.');
        if (dot < 0) dot = path.Length;
        return path.Substring(slash, dot - slash);
    }
}