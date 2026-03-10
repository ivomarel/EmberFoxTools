using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomSettings<T> : ScriptableObject where T : CustomSettings<T>
{
    public BuildTarget activeForBuildTarget;

    public enum BuildTarget
    {
        Any,
        Editor,
        MobileBuild
    }

    private static string AssetName => typeof(T).Name;
    private static string Path => BasePath + AssetName + ".asset";
    private static string BasePath => "Assets/CustomSettings/Resources/";

    private static T cachedSettings;

    public static T Get
    {
        get
        {
            if (cachedSettings == null)
            {
                var settings = Resources.LoadAll<T>("");

                foreach (var setting in settings)
                {
                    switch (setting.activeForBuildTarget)
                    {
                        case BuildTarget.Any:
                            cachedSettings = setting;
                            break;
                        case BuildTarget.Editor:
                            if (Application.isEditor)
                            {
                                cachedSettings = setting;
                            }
                            break;
                        case BuildTarget.MobileBuild:
                            if (!Application.isEditor)
                            {
                                cachedSettings = setting;
                            }
                            break;
                    }
                }
            }

#if UNITY_EDITOR
            if (cachedSettings == null)
            {
                cachedSettings = CreateCustomSettings();
            }
#endif

            return cachedSettings;
        }
    }

#if UNITY_EDITOR
    private static T CreateCustomSettings()
    {
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }
        
        var path = Path;
        while (File.Exists(path))
        {
            //TODO Improve this
            path = path.Replace(".asset", "_1.asset");
        }
        
        T soAsset = CreateInstance<T>();
        AssetDatabase.CreateAsset(soAsset, path);
        AssetDatabase.SaveAssets();
        UnityEngine.Debug.Log("Created " + soAsset.name);

        return soAsset;
        
    }
#endif
}