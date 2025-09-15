using System.Collections.Generic;
using Dark.Scripts.CoreUI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIUtility
{
    public static void SetAlpha(this Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
    
    /// <summary>
    /// Check if the mouse is truly exit an image, it doesn't exit even the mouse is moved to an overlapped area between 2 images
    /// </summary>
    /// <returns></returns>
    public static bool PointerStillOverMe(this RectTransform rect, Canvas canvas)
    {
        var cam = canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        // Quick rect test (fast)
        var insideRect = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, cam);
        if (!insideRect) return false;

        // Optional: robust check using a UI raycast for all hits at pointer
        // to ensure this object is actually hit by the UI system (not masked out, etc.)
        var es = EventSystem.current;
        if (es == null) return insideRect;

        var ped = new PointerEventData(es) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        es.RaycastAll(ped, results);

        foreach (var r in results)
        {
            if (r.gameObject == rect.gameObject) return true;
        }
        return false;
    }
}