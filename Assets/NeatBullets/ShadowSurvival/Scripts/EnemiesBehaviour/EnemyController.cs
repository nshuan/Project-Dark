using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.SODefinitions;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour {
    public class EnemyController : MonoBehaviour, IDamagable
    {
        public float HealthPoints { get; set; }
        public float MovementSpeed;
        public float AttackDistance;
    
        public Rigidbody2D Rigidbody;
        public Animator Animator;
        public EnemyWeapon Weapon;
        public SpriteRenderer SpriteRenderer;
        public CapsuleCollider2D Collider;
        public GameObject Player;
    
        public EnemyStateMachine StateMachine;
    
        // Assigned from the Editor
        public GlobalVariablesSO CommonVariables;
        public EnemyProjectile ProjectilePrefab;
    
        private void Awake() {
            HealthPoints = Random.Range(10f, 30f);
            MovementSpeed = Random.Range(2.5f, 3.5f);
            AttackDistance = Random.Range(6.5f, 9f);
        
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Weapon = GetComponentInChildren<EnemyWeapon>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Collider = GetComponent<CapsuleCollider2D>();
            Player = GameObject.Find("Player");
        
            StateMachine = new EnemyStateMachine(this);
        }
        private void Start() {
            StateMachine.Initialize(StateMachine.Awake);
        }
        private void Update() {
            StateMachine.Update();
        }
        private void FixedUpdate() {
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




