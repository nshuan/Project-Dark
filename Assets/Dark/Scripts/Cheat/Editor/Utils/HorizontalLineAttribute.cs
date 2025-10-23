using UnityEngine;

public class HorizontalLineAttribute : PropertyAttribute
{
    public readonly float Height;
    public readonly float Padding;
    public readonly Color Color;

    public HorizontalLineAttribute(
        float height = 1f, float padding = 6f,
        float r = 0.3f, float g = 0.3f, float b = 0.3f,
        int order = 0)
    {
        Height = height;
        Padding = padding;
        Color = new Color(r, g, b, 1f);
        this.order = order; // <-- key line
    }
}