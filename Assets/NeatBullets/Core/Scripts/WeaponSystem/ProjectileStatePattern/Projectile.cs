using System;
using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.SpaceShooter.Scripts.Managers;
using SharpNeat.Phenomes;
using Unity.Mathematics;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern {
    public class Projectile : MonoBehaviour, IDamagable
    {
        // assign in editor
        [SerializeField] public GlobalVariablesSO GlobalVariables;
        
        // assigned from Abstract Weapon
        [SerializeField] public WeaponParams WeaponParamsLocal;
        [SerializeField] public WeaponParamsSO WeaponSo;
        [HideInInspector] public Transform OriginTransform;
        public float SignX;
        public float SignY;
        public Vector2 InitialVelocity;
        public IBlackBox Box;
        
        
        [HideInInspector] public Rigidbody2D Rigidbody;
        [HideInInspector] public SpriteRenderer SpriteRenderer;
        [HideInInspector] public TrailRenderer TrailRenderer;
        // StartPoint for RayCast 
        private Transform _tipTransform;
        
        public float BirthTime;
        
        [SerializeField] public ProjectileStateMachine StateMachine;
        private void Awake() {
            Rigidbody = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            TrailRenderer = GetComponent<TrailRenderer>();
            _tipTransform = transform.Find("Tip");
            
            BirthTime = Time.time;
            
            StateMachine = new ProjectileStateMachine(this);
        }

        private ISignalArray _inputArr;
        private ISignalArray _outputArr;
        private void Start() {
            _inputArr = Box.InputSignalArray;
            _outputArr = Box.OutputSignalArray;
            transform.localScale = WeaponParamsLocal.Size;
            InitializeTrail();
            
            StateMachine.Initialize(StateMachine.InitialFlight);

            if (WeaponParamsLocal.HasLifespan) 
                DestroyItselfCoroutine = StartCoroutine(DestroyItself(WeaponParamsLocal.Lifespan));
            
            WeaponSo.DestroyProjectilesEvent += DestroyItselfNow;
        }
        
        private void OnDestroy() {
            WeaponSo.DestroyProjectilesEvent -= DestroyItselfNow;
        }
        
        private void InitializeTrail() {
           if (WeaponParamsLocal.HasTrail) {
                TrailRenderer.startWidth = WeaponParamsLocal.Size.x;
                TrailRenderer.endWidth = 0f;
                _prevColor = Color.white;
                TrailRenderer.time = 0.1f;
           }
           else 
               TrailRenderer.emitting = false;
        }
        
        private void DestroyItselfNow() {
            Destroy(OriginTransform.gameObject);
        }
        
        public Coroutine DestroyItselfCoroutine;
        public IEnumerator DestroyItself(float delay) {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
        
        public void ActivateBlackBoxCircle() {
            Box.ResetState();

            float maxDistance = WeaponParamsLocal.NNControlDistance * math.SQRT2;
            
            _inputArr[0] = Mathf.Lerp(-1f, 1f,Math.Abs(RelativePos.x) / WeaponParamsLocal.NNControlDistance);
            _inputArr[1] = Mathf.Lerp(-1f, 1f,Math.Abs(RelativePos.y) / WeaponParamsLocal.NNControlDistance);
            _inputArr[2] = Mathf.Lerp(-1f, 1f,DistanceFromOrigin / maxDistance);
            
            Box.Activate();
        }
        
        public void ActivateBlackBoxRect() {
            Box.ResetState();
            
            float maxX = WeaponParamsLocal.RectDimensions.x * WeaponParamsLocal.NNControlDistance;
            float maxY = WeaponParamsLocal.RectDimensions.y * WeaponParamsLocal.NNControlDistance;
            float maxDistance = maxX > maxY ? maxX : maxY;
            
            _inputArr[0] = Mathf.Lerp(-1f, 1f,Math.Abs(RelativePos.x) / maxX);
            _inputArr[1] = Mathf.Lerp(-1f, 1f,Math.Abs(RelativePos.y) / maxY);
            _inputArr[2] = Mathf.Lerp(-1f, 1f,DistanceFromOrigin / maxDistance);
            
            Box.Activate();
        }
        
        public void ActivateBlackBoxPolar() {
            Box.ResetState();
            
            float maxPhiRad = WeaponParamsLocal.MaxPolarAngleDeg * Mathf.Deg2Rad;
            float NNControlDistance = WeaponParamsLocal.NNControlDistance;
            
            // Remember: PhiRad is an angle between polar radius and Y axis (not X axis like in standard case). This is done for symmetry of flight pattern.
            // Therefore, these formulas are correct
            float x = Math.Abs( DistanceFromOrigin * Mathf.Sin(PhiRad) );
            float y = Math.Abs( DistanceFromOrigin * Mathf.Cos(PhiRad) );
            
            float maxX = maxPhiRad * Mathf.Rad2Deg >= 90f ? NNControlDistance : NNControlDistance * Mathf.Sin(maxPhiRad);
            
            _inputArr[0] = Mathf.Lerp(-1f, 1f,x / maxX);
            _inputArr[1] = Mathf.Lerp(-1f, 1f,y / NNControlDistance);
            _inputArr[2] = Mathf.Lerp(-1f, 1f,DistanceFromOrigin / NNControlDistance);
        
            Box.Activate();
        }
        
        private float _hue, _maxSpeed, _force;
        private Vector2 _forceDir;
        public void ReadDataFromBlackBox() {
            float x = Mathf.Lerp(-1f, 1f, (float)_outputArr[0]) * SignX;
            float y = Mathf.Lerp(-1f, 1f, (float)_outputArr[1]) * SignY;

            switch (WeaponParamsLocal.ReadMode) {
                case ReadMode.Default:
                    _forceDir = OriginTransform.TransformDirection(x, y, 0f);
                    break;
                
                case ReadMode.Rotator:
                    Vector2 rotatingDir = Vector2.Perpendicular(RelativePosDir).normalized;
                    _forceDir = x * rotatingDir + y * RelativePosDir.normalized;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _hue = Mathf.Lerp(WeaponParamsLocal.HueRange.x, WeaponParamsLocal.HueRange.y, (float)_outputArr[2]);
            _maxSpeed = Mathf.Lerp(WeaponParamsLocal.SpeedRange.x, WeaponParamsLocal.SpeedRange.y, (float)_outputArr[3]);
            _force = Mathf.Lerp(WeaponParamsLocal.ForceRange.x, WeaponParamsLocal.ForceRange.y, (float)_outputArr[4]);

            SpriteRenderer.color = Color.HSVToRGB(_hue, WeaponParamsLocal.Saturation, WeaponParamsLocal.Brightness);
            
            switch (WeaponParamsLocal.NetworkControlMode) {
                case NetworkControlMode.ForceSum:
                    Rigidbody.AddForce(_forceDir * _force);
                    if (WeaponParamsLocal.ForwardForce) 
                        Rigidbody.AddForce(OriginTransform.up * _force);
                    break;
                
                case NetworkControlMode.VelocitySum:
                    float inverseMass = 1f / Rigidbody.mass;
                    Rigidbody.velocity += _forceDir * (_force * inverseMass * Time.fixedDeltaTime);
                    if (WeaponParamsLocal.ForwardForce)
                        Rigidbody.velocity += (Vector2)OriginTransform.up * (_force * inverseMass * Time.fixedDeltaTime);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        public void LimitMaxSpeed() {
            float speed = Rigidbody.velocity.magnitude;
            if (speed > _maxSpeed)
                Rigidbody.velocity = Rigidbody.velocity.normalized * _maxSpeed;
        }

        private void Update() {
            CalcProjectileStats();
            StateMachine.Update();
        }
        
        private Color _prevColor;
        private Color _curColor;
        private void FixedUpdate() {
            CheckCollision();
            StateMachine.FixedUpdate();
            
            if (WeaponParamsLocal.HasTrail) {
                _curColor = SpriteRenderer.color;
                TrailRenderer.startColor = _curColor;
                TrailRenderer.endColor = new Color(_prevColor.r, _prevColor.g, _prevColor.b, 0);
                _prevColor = _curColor;
            }
            
            transform.up = Rigidbody.velocity;
        }

        private void LateUpdate() {
            // Transition to PauseState is common to any state, so we check it here
            // Do not transit if it's already in paused state
            if (GlobalVariables.IsPaused && StateMachine.CurrentState != StateMachine.Pause && gameObject.layer != 5) // layer 5: UI
                StateMachine.TransitionTo(StateMachine.Pause);
            
            StateMachine.LateUpdate();
        }
        
        
        public Vector2 RelativePos;
        public Vector2 RelativePosDir;
        public float DistanceFromOrigin;
        public float PhiRad;
        private void CalcProjectileStats() {
            RelativePosDir = transform.position - OriginTransform.position;
            RelativePos = transform.localPosition;
            DistanceFromOrigin = RelativePos.magnitude;
            PhiRad = Vector2.Angle(OriginTransform.up, RelativePosDir) * Mathf.Deg2Rad;
        }
        
        
        [SerializeField] private int _layerMask = 0b_0000_0000_1000;
        protected void CheckCollision() {
            Vector2 startPoint = _tipTransform.position;
            RaycastHit2D hit = Physics2D.CircleCast(startPoint, transform.localScale.x, -transform.up, transform.localScale.y, _layerMask);
            
            if (ReferenceEquals(hit.collider, null))
                return;
        
            // For projectiles HP acts like Damage
            IDamagable objectToDamage = hit.transform.GetComponent<IDamagable>();
            GameManager.Instance.DamageObject(objectToDamage, HealthPoints);
            
            // Projectile damages itself on collision
            if (objectToDamage.HealthPoints > 0)
                GameManager.Instance.DamageObject(this, HealthPoints);
        }
        
        
        [field: SerializeField] public float HealthPoints { get; set; } = 2f;
        public void TakeDamage(float damage) {
            HealthPoints -= damage;
            if (HealthPoints <= 0)
                Destroy(gameObject);
        }
        

        private void OnDrawGizmosSelected() {
            if (OriginTransform == null)
                return;
            Gizmos.DrawRay(OriginTransform.position, RelativePosDir);
        }
        
    }
}
