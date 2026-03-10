using UnityEngine;

public enum FieldButtonPosition
{
    Above,
    InlineLeft,
    InlineRight
}

/// <summary>
/// Draws a button next to or above a field in the inspector.
/// When clicked, calls a parameterless method on the target object.
/// </summary>
public class FieldButtonAttribute : PropertyAttribute
{
    public readonly string CallbackName;
    public readonly string Label;
    public readonly FieldButtonPosition Position;

    /// <param name="callbackName">Name of a method to invoke (use nameof(...)).</param>
    /// <param name="label">Optional custom button label.</param>
    /// <param name="position">Button placement: Above, InlineLeft, InlineRight.</param>
    public FieldButtonAttribute(string callbackName, string label = null, FieldButtonPosition position = FieldButtonPosition.InlineRight)
    {
        CallbackName = callbackName;
        Label = label;
        Position = position;
    }
}