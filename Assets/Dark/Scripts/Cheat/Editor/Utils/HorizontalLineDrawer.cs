using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
public class HorizontalLineDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        var lineAttr = (HorizontalLineAttribute)attribute;

        position.y += lineAttr.Padding / 2;
        position.height = lineAttr.Height;

        EditorGUI.DrawRect(position, lineAttr.Color);
    }

    public override float GetHeight()
    {
        var lineAttr = (HorizontalLineAttribute)attribute;
        return lineAttr.Height + lineAttr.Padding;
    }
}