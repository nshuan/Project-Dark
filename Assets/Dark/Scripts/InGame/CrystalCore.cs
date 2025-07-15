using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class CrystalCore : MonoBehaviour
    {
        [Header("Pulse Settings")]
        public float pulseSpeed = 2f;
        public float pulseAmount = 0.1f;
        private Vector3 baseScale;

        [Header("Flicker Settings")]
        public SpriteRenderer spriteRenderer; // Use SpriteRenderer for 2D
        public Color emissionColor = Color.cyan;
        public float flickerMin = 0.5f;
        public float flickerMax = 2f;
        public float flickerSpeed = 10f;

        private Material material;
        private float flickerTime;

        void Start()
        {
            baseScale = transform.localScale;

            if (spriteRenderer != null)
            {
                // Clone material to avoid affecting shared material
                material = spriteRenderer.material;
            }
        }

        void Update()
        {
            // --- PULSE ---
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale * scale;

            // --- FLICKER ---
            if (material != null)
            {
                flickerTime += Time.deltaTime * flickerSpeed;
                float flicker = Mathf.Lerp(flickerMin, flickerMax, (Mathf.Sin(flickerTime) + 1f) / 2f);
                material.color = emissionColor * flicker;
            }
        }
    }
}