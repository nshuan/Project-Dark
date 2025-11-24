using UnityEngine;

namespace Economic.InGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GlowEffect : MonoBehaviour
    {
        [Header("Glow Settings")]
        [SerializeField] private float glowSpeed = 2f;
        [SerializeField] private float minAlpha = 0.3f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private bool useColorTint = false;
        [SerializeField] private Color glowColor = Color.white;
        
        [Header("Easing")]
        [SerializeField] private AnimationCurve glowCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private float _time;
        private bool _isInitialized = false;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
                _isInitialized = true;
            }
            else
            {
                _isInitialized = false;
                Debug.LogWarning($"GlowEffect: SpriteRenderer component not found on {gameObject.name}");
            }
            
            _time = 0f;
        }

        private void OnEnable()
        {
            _time = 0f;
            if (_isInitialized)
            {
                // Reset to original color when enabled
                _spriteRenderer.color = _originalColor;
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;
            
            _time += Time.deltaTime * glowSpeed;
            
            // Calculate normalized value (0 to 1) using sine wave
            float normalizedValue = (Mathf.Sin(_time) + 1f) * 0.5f;
            
            // Apply curve if provided
            if (glowCurve != null && glowCurve.length > 0)
            {
                normalizedValue = glowCurve.Evaluate(normalizedValue);
            }
            
            // Interpolate alpha between min and max
            float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, normalizedValue);
            
            // Apply the glow effect
            Color currentColor = useColorTint ? glowColor : _originalColor;
            currentColor.a = currentAlpha;
            _spriteRenderer.color = currentColor;
        }

        private void OnDisable()
        {
            if (_isInitialized && _spriteRenderer != null)
            {
                // Reset to original color when disabled
                _spriteRenderer.color = _originalColor;
            }
            _time = 0f;
        }

        /// <summary>
        /// Sets the glow speed
        /// </summary>
        public void SetGlowSpeed(float speed)
        {
            glowSpeed = speed;
        }

        /// <summary>
        /// Sets the alpha range for the glow effect
        /// </summary>
        public void SetAlphaRange(float min, float max)
        {
            minAlpha = Mathf.Clamp01(min);
            maxAlpha = Mathf.Clamp01(max);
        }

        /// <summary>
        /// Sets the glow color (only applies if useColorTint is enabled)
        /// </summary>
        public void SetGlowColor(Color color)
        {
            glowColor = color;
        }

        /// <summary>
        /// Resets the glow effect to its original state
        /// </summary>
        public void ResetGlow()
        {
            _time = 0f;
            if (_isInitialized && _spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }
    }
}

