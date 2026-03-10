#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
        SerializedProperty scenePath = property.FindPropertyRelative("scenePath");
        SerializedProperty sceneName = property.FindPropertyRelative("sceneName");

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, sceneAsset, label);
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
        
        if (EditorGUI.EndChangeCheck())
        {
            if (sceneAsset.objectReferenceValue != null)
            {
                string path = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                scenePath.stringValue = path;
                sceneName.stringValue = System.IO.Path.GetFileNameWithoutExtension(path);
            }
            else
            {
                scenePath.stringValue = string.Empty;
                sceneName.stringValue = string.Empty;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        
        //GUILayout.Label(scenePath.stringValue);
        //GUILayout.Label(sceneName.stringValue);
    }
}
#endif