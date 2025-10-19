using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid;

/// <summary>
/// Draws multiple 2D lines inside a Canvas (works in Screen Space and World Space canvases).
/// Each line can have multiple points.
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class UIMultiLineRenderer : MaskableGraphic
{
    [System.Serializable]
    public class Line
    {
        public List<Vector2> points = new List<Vector2>();
    }

    public List<Line> lines = new List<Line>();

    private Action onUpdateLines;

    public float LineThickness;
    public Color LineColor;
    
    protected override void Awake()
    {
        base.Awake();

        onUpdateLines = null;
        onUpdateLines += SetVerticesDirty;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (lines == null || lines.Count == 0)
            return;

        foreach (var line in lines)
        {
            if (line.points == null || line.points.Count < 2)
                continue;

            for (int i = 0; i < line.points.Count - 1; i++)
            {
                Vector2 start = line.points[i];
                Vector2 end = line.points[i + 1];
                DrawLine(vh, start, end, LineColor);
            }
        }
    }

    void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, Color col)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (LineThickness / 2f);

        UIVertex v0 = UIVertex.simpleVert;
        UIVertex v1 = UIVertex.simpleVert;
        UIVertex v2 = UIVertex.simpleVert;
        UIVertex v3 = UIVertex.simpleVert;

        v0.color = v1.color = v2.color = v3.color = col;

        v0.position = start - normal;
        v1.position = start + normal;
        v2.position = end + normal;
        v3.position = end - normal;

        vh.AddUIVertexQuad(new[] { v0, v1, v2, v3 });
    }

    public void DrawLine(Vector2 start, Vector2 end)
    {
        lines ??= new List<Line>();
        lines.Add(new Line() { points = new List<Vector2>() { start, end } });
        onUpdateLines?.Invoke();
    }

    public void ForceUpdateLines()
    {
        onUpdateLines?.Invoke();
    }

    public void Clear()
    {
        lines.Clear();
    }
}