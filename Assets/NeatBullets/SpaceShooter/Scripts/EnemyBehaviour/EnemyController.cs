using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern;
using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.EnemyBehaviour {
    public class EnemyController : MonoBehaviour, IDamagable
    {
        public float HealthPoints { get; set; }
        public float MovementSpeed;
        public float AttackDistance;
    
        public Rigidbody2D Rigidbody;
        public EnemyWeapon Weapon;
        public CircleCollider2D Collider;
        public SpriteRenderer Renderer;
        public AudioSource AudioSource;
        public GameObject Player;
    
        public EnemyStateMachine StateMachine;
    
        // Assign in editor
        public GlobalVariablesSO GlobalVariables;
        public Projectile ProjectilePrefab;
        public Sprite Sprite;
        
        public Shader ShaderGUItext;
        public Shader ShaderSpritesDefault;
        
        private void Awake() {
            HealthPoints = 16f;
            MovementSpeed = 2.5f;
            AttackDistance = 5.5f;
        
            Rigidbody = GetComponent<Rigidbody2D>();
            Weapon = GetComponentInChildren<EnemyWeapon>();
            Collider = GetComponent<CircleCollider2D>();
            Renderer = GetComponent<SpriteRenderer>();
            AudioSource = GetComponent<AudioSource>();
            Player = GameObject.Find("Player");
            
            ShaderGUItext = Shader.Find("GUI/Text Shader");
            ShaderSpritesDefault = Renderer.material.shader;
        
            StateMachine = new EnemyStateMachine(this);
        }
        
        private void Start() {
            Renderer.sprite = Sprite;
            StateMachine.Initialize(StateMachine.ChasingPlayer);
        }
        
        private void Update() {
            StateMachine.Update();
        }

        
        private void FixedUpdate() {
            Vector2 playerDir = (Player.transform.position - transform.position).normalized;
            transform.up = playerDir;
            StateMachine.FixedUpdate();
        }
        
        private void LateUpdate() {
            StateMachine.LateUpdate();
        }
    
        public void TakeDamage(float damage) {
            StateMachine.TransitionTo(StateMachine.Hurt);
            Rigidbody.velocity = Vector2.zero;
            HealthPoints -= damage;
        }
    
    
    
    
    }
}




