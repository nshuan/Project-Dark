using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.SODefinitions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeatBullets.SpaceShooter.Scripts.Managers {
    
    // Enemy spawning, Pause and Death canvases activation, invoking of TakeDamage
    public class GameManager : MonoBehaviour {

        // Assign in editor
        [SerializeField] private GlobalVariablesSO _globalVariables;
        
        public static GameManager Instance;
        [HideInInspector] [SerializeField] private Camera _camera;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
            _camera = Camera.main;
        }
        
        

        [SerializeField] private GameObject _pauseCanvas;
        [SerializeField] private GameObject _deathCanvas;
        private InputManager _inputManager;
        private InputAction _pauseAction;
        private void Initialize() {
            _inputManager = InputManager.Instance;
            _pauseAction = _inputManager.PauseAction;
        }

        private void Start() {
            Initialize();
            _pauseAction.started += ActivatePauseCanvas;
        }

        private void OnDestroy() {
            _pauseAction.started -= ActivatePauseCanvas;
        }
        
        
        /// <summary>
        /// Activate and deactivate pause canvas on user input (Esc on PC or Back on mobile)
        /// </summary>
        private void ActivatePauseCanvas(InputAction.CallbackContext context) {
            if (!_pauseCanvas.activeSelf) {
                _pauseCanvas.SetActive(true);
                Pause(true);
            }
            else {
                _pauseCanvas.SetActive(false);
                Pause(false);
            }
        }
        
        // Function for resume button
        public void ActivatePauseCanvas() {
            if (!_pauseCanvas.activeSelf) {
                _pauseCanvas.SetActive(true);
                Pause(true);
            }
            else {
                _pauseCanvas.SetActive(false);
                Pause(false);
            }
        }

        /// <summary>
        /// Change value isPaused in GlobalVariables
        /// </summary>
        public void Pause(bool value) {
            _globalVariables.IsPaused = value;
        }

        /// <summary>
        /// Invoke TakeDamage method of IDamagable object, if its HPs are nonnegative
        /// </summary>
        public void DamageObject(IDamagable objectToDamage, float damage) {
            if (objectToDamage.HealthPoints <= 0f)
                return;
            objectToDamage.TakeDamage(damage);
        }
        
        
        public void ActivateDeathCanvas() {
            if (!_deathCanvas.activeSelf) {
                _deathCanvas.SetActive(true);
                Pause(true);
            }
            else {
                _deathCanvas.SetActive(false);
                Pause(false);
            }
        }

        











    }
}
