using UnityEngine;

public class CollapsableAttribute : PropertyAttribute
{
    public string Label;

    public CollapsableAttribute(string label = null)
    {
        Label = label;
    }
}