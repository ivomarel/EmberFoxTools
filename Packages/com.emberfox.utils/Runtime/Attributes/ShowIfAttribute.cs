using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionFieldName; // The name of the field that will control visibility
    public object[] compareValues; // The value to compare against

    public ShowIfAttribute(string conditionFieldName, params object[] compareValues)
    {
        this.conditionFieldName = conditionFieldName;
        this.compareValues = compareValues;
    }
}

public class DontShowIfAttribute : ShowIfAttribute
{

    public DontShowIfAttribute(string conditionFieldName, params object[] compareValues) : base(conditionFieldName, compareValues)
    {
        
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute) attribute;

        // Get the condition field
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionFieldName);

        if (conditionProperty != null)
        {
            // Check if the condition is met
            if (IsConditionMet(conditionProperty, showIf.compareValues))
            {
                // Show the field if the condition is met
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            Debug.LogWarning($"Condition field '{showIf.conditionFieldName}' not found in object: {property.serializedObject.targetObject}");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute) attribute;

        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionFieldName);

        if (conditionProperty != null && IsConditionMet(conditionProperty, showIf.compareValues))
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        return 0; // If the condition is not met, don't take up any space
    }

    protected virtual bool IsConditionMet(SerializedProperty conditionProperty, object[] compareValues)
    {
        foreach (object compareValue in compareValues)
        {
            switch (conditionProperty.propertyType)
            {
                case SerializedPropertyType.Enum:
                    if (conditionProperty.enumValueIndex == (int) compareValue)
                        return true;
                    break;
                case SerializedPropertyType.Boolean:
                    if (conditionProperty.boolValue == (bool) compareValue)
                        return true;
                    break;
                case SerializedPropertyType.String:
                    if (conditionProperty.stringValue == (string) compareValue)
                        return true;
                    break;
                case SerializedPropertyType.ObjectReference:
                    if (conditionProperty.objectReferenceValue != null == (bool) compareValue)
                        return true;
                    break;
                // Add other cases if necessary (float, int, etc.)
                default:
                    Debug.LogWarning($"Unsupported property type '{conditionProperty.propertyType}' for condition.");
                    return false;
            }
        }
        return false;
    }
}

[CustomPropertyDrawer(typeof(DontShowIfAttribute))]
public class DontShowIfDrawer : ShowIfDrawer
{
    protected override bool IsConditionMet(SerializedProperty conditionProperty, object[] compareValues)
    {
        // Invert the logic of the base class
        return !base.IsConditionMet(conditionProperty, compareValues);
    }
}


#endif