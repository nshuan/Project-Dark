using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.SpaceShooter.Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeatBullets.ShadowSurvival.Scripts.Player {
    public class Player : MonoBehaviour, IDamagable
    {
        // assign in the editor
        [SerializeField] private GlobalVariablesSO _globalVariables;

        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private GameWeapon _weapon;
        
        [HideInInspector] [SerializeField] private Shader _shaderGUItext;
        [HideInInspector] [SerializeField] private Shader _shaderSpritesDefault;
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _weapon = GetComponentInChildren<GameWeapon>();
            
            _shaderGUItext = Shader.Find("GUI/Text Shader");
            _shaderSpritesDefault = _spriteRenderer.material.shader;
        }

        private InputManager _inputManager;
        private InputAction _moveAction;
        private InputAction _aimAction;
        
        [field: SerializeField] [field: HideInInspector] [field: Range(10f, 100f)] public float HealthPoints { get; set; }
        [field: SerializeField] [field: HideInInspector] [field: Range(2f, 5.5f)] public float Speed { get; set; }
        private void Initialize() {
            _inputManager = InputManager.Instance;
            _moveAction = _inputManager.PlayerActionMap.moveAction;
            _aimAction = _inputManager.PlayerActionMap.aimAction;
            
            Speed = 4f;
            HealthPoints = 10f;
            _currPos = transform.position;
        }

        
        private void Start() {
            Initialize();
            
            _aimAction.started += StartShooting;
            _aimAction.canceled += StopShooting;
            _globalVariables.OnPauseResumeEvent += OnPauseResumeGame;
        }

        private void OnDestroy() {
            _aimAction.started -= StartShooting;
            _aimAction.canceled -= StopShooting;
            _globalVariables.OnPauseResumeEvent -= OnPauseResumeGame;
        }

        private void StartShooting(InputAction.CallbackContext context) {
            Speed = 3f;
            _weapon.FireCoroutine = _weapon.StartCoroutine(_weapon.Fire());
        }
        
        private void StopShooting(InputAction.CallbackContext context) {
            Speed = 4f;
            _weapon.StopAllCoroutines();
        }
        
        private void OnPauseResumeGame() {
            _rigidbody.velocity = Vector2.zero;
            _animator.enabled = !_globalVariables.IsPaused;
        }
        

        
        
        private Vector2 _move { get; set; }
        private Vector2 _aim { get; set; }
        private void Update() {
            _move = _moveAction.ReadValue<Vector2>().normalized;
            _aim = _aimAction.ReadValue<Vector2>().normalized;
        }
        
        
    
        private Vector2 _prevPos;
        private Vector2 _currPos;
        [HideInInspector] public Vector2 ActualVelocity;
        private void FixedUpdate() {
            if (_globalVariables.IsPaused)
                return;
            
            _rigidbody.velocity = _move * Speed;
            _animator.SetFloat("speed", _rigidbody.velocity.magnitude);

            if (_aim != Vector2.zero) 
                _weapon.transform.right = _aim;
            
        
            if (HealthPoints <= 0)
                OnDeath();
            
            
            // ActualVelocity is needed for MapReposition
            // Using rigidbody.velocity is not reliable
            _prevPos = _currPos;
            _currPos = transform.position;
            ActualVelocity = (_currPos - _prevPos) / Time.fixedDeltaTime;
        }
        

        private void LateUpdate() {
            if (_move.x != 0) {
                _spriteRenderer.flipX = _move.x < 0;
            } else if (_aim != Vector2.zero) {
                _spriteRenderer.flipX = _aim.x < 0;
            }
        }
        
        private IEnumerator ChangeShader(Shader shader, float delay) {
            yield return new WaitForSeconds(delay);
            _spriteRenderer.material.shader = shader;
            _spriteRenderer.color = Color.white;
        }
        
        
        public void TakeDamage(float damage) {
            StartCoroutine(ChangeShader(_shaderGUItext,0f));
            HealthPoints -= damage;
            StartCoroutine(ChangeShader(_shaderSpritesDefault,0.2f));
        }
        
        private void OnDeath() {
            GameManager.Instance.ActivateDeathCanvas();
        }
    
    }
}
