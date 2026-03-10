using UnityEngine;
using UnityEditor;

public class PoorTextAreaAttribute : PropertyAttribute
{
    public int minLines;
    public int maxLines;

    public PoorTextAreaAttribute(int minLines = 3, int maxLines = 5)
    {
        this.minLines = minLines;
        this.maxLines = maxLines;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PoorTextAreaAttribute))]
public class PoorTextAreaDrawer : PropertyDrawer
{
    float textAreaWidth;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Label
        position = EditorGUI.PrefixLabel(position, label);

        textAreaWidth = position.width;

        Rect textRect = new Rect(position.x, position.y, position.width, GetAreaHeight(property));
        // Our text
        property.stringValue = EditorGUI.TextArea(textRect, property.stringValue, GetStyle());

        EditorGUI.EndProperty();
    }

    private float GetAreaHeight(SerializedProperty property)
    {
        PoorTextAreaAttribute area = (PoorTextAreaAttribute) attribute;
        // Text height
        GUIContent guiContent = new GUIContent(property.stringValue);
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float textHeight = Mathf.Clamp(GetStyle().CalcHeight(guiContent, textAreaWidth), area.minLines * lineHeight, area.maxLines * lineHeight);
        return textHeight;
    }

    private GUIStyle GetStyle()
    {
        // Custom style
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.richText = false;
        textAreaStyle.wordWrap = true;
        return textAreaStyle;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetAreaHeight(property);
    }
}
#endif