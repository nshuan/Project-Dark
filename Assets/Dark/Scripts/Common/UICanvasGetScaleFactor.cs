using System;
using UnityEngine;

namespace Dark.Scripts.Common
{
    public class UICanvasGetScaleFactor : MonoBehaviour
    {
        public static float scaleFactor = 1f;

        private void Awake()
        {
            if (TryGetComponent<Canvas>(out var canvas))
                scaleFactor = canvas.scaleFactor;
        }
    }
}