using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [ReadOnly] public string scenePath;
    [ReadOnly] public string sceneName;

    public bool IsSet => !string.IsNullOrEmpty(scenePath);

}