using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dark.Scripts.Utils.Camera
{
    /// <summary>
    /// If both conformX and conformY are true, skip the default ratio
    /// </summary>
    public class SafeScaler : MonoBehaviour
    {
        [SerializeField] private Vector2 defaultResolution = new Vector2(1920, 1080);
        [SerializeField] private bool conformX;
        [SerializeField] private bool conformY;

        private void Awake()
        {
            UpdateScale();
        }

        // private void OnValidate()
        // {
        //     UpdateScale();
        // }

        public void UpdateScale()
        {
            var rectTransform = GetComponent<RectTransform>();
            var size = new Vector2(defaultResolution.x, defaultResolution.y);

            if (conformX && conformY)
            {
                size.x = Screen.currentResolution.width;
                size.y = Screen.currentResolution.height;
            }
            else if (conformX)
            {
                var ratio = size.y / size.x;
                size.x = Screen.currentResolution.width;
                size.y = size.x * ratio;
            }
            else if (conformY)
            {
                var ratio = size.x / size.y;
                size.y = Screen.currentResolution.height;
                size.x = size.y * ratio;
            }
            else
            {
                size = defaultResolution;
            }
            
            rectTransform.sizeDelta = size;
        }
    }

#if UNITY_EDITOR
    // [InitializeOnLoad]
    // public static class SafeScalerUpdateEditor
    // {
    //     private static Vector2 lastGameViewAspectRatio;
    //     
    //     static SafeScalerUpdateEditor()
    //     {
    //         EditorApplication.update += CheckGameViewSize;
    //     }
    //
    //     static void CheckGameViewSize()
    //     {
    //         Vector2 currentSize = Handles.GetMainGameViewSize();
    //         if (currentSize != lastGameViewAspectRatio)
    //         {
    //             lastGameViewAspectRatio = currentSize;
    //             var scalers = Object.FindObjectsOfType<SafeScaler>();
    //             foreach (var scaler in scalers)
    //             {
    //                 scaler.UpdateScale();
    //             }
    //             Debug.Log($"GameView size changed: {currentSize}");
    //         }
    //     }
    // }
#endif
}
