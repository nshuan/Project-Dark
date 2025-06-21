using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class MonoCursor : MonoBehaviour
    {
        [SerializeField] private Image cooldown;

        private RectTransform rectCursor;

        private void Awake()
        {
            rectCursor = GetComponent<RectTransform>();
        }

        public Image UICooldown => cooldown;
    }
}