using UnityEngine;

namespace Economic.InGame
{
    public class FloatingEffect : MonoBehaviour
    {
        [Header("Floating Settings")]
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatAmplitude = 0.5f;
        [SerializeField] private Vector3 floatDirection = Vector3.up;
        
        [Header("Rotation Settings")]
        [SerializeField] private bool enableRotation = false;
        [SerializeField] private Vector3 rotationSpeed = Vector3.zero;
        
        [Header("Offset")]
        [SerializeField] private Vector3 startOffset = Vector3.zero;
        
        private Vector3 _startPosition;
        private Vector3 _basePosition;
        private float _time;
        private bool _isPaused;

        private void Awake()
        {
            _startPosition = transform.localPosition;
            _basePosition = _startPosition + startOffset;
            _time = 0f;
        }

        private void Update()
        {
            if (_isPaused) return;
            _time += Time.deltaTime;
            
            // Calculate floating position using sine wave
            float floatingValue = Mathf.Sin(_time * floatSpeed) * floatAmplitude;
            Vector3 floatingOffset = floatDirection.normalized * floatingValue;
            transform.localPosition = _basePosition + floatingOffset;
            
            // Apply rotation if enabled
            if (enableRotation)
            {
                transform.Rotate(rotationSpeed * Time.deltaTime);
            }
        }

        private void OnDisable()
        {
            // Reset to base position when disabled
            transform.localPosition = _basePosition;
            _time = 0f;
        }

        /// <summary>
        /// Resets the floating effect to the original position
        /// </summary>
        public void ResetPosition()
        {
            _startPosition = transform.localPosition;
            _basePosition = _startPosition + startOffset;
            _time = 0f;
        }

        public void PauseFloat()
        {
            _isPaused = true;
        }

        public void ResumeFloat()
        {
            _isPaused = false;
        }
    }
}

