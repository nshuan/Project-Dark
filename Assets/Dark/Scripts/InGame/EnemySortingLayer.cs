using System;
using UnityEngine;

namespace InGame
{
    public class EnemySortingLayer : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private int baseSortingOrder;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseSortingOrder = spriteRenderer.sortingOrder;
        }

        void LateUpdate()
        {
            // Multiply by -100 or another scale to convert Y float to int
            spriteRenderer.sortingOrder = baseSortingOrder + Mathf.RoundToInt(transform.position.y * -100);
        }
    }
}