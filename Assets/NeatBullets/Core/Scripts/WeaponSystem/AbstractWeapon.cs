using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem.NEAT;
using NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeatBullets.Core.Scripts.WeaponSystem {
    public abstract class AbstractWeapon : MonoBehaviour
    {
        [SerializeField] protected WeaponParamsSO _weaponSO;
        public Projectile ProjectilePrefab;
        public CoordinateSystem CoordinateSystemPrefab;
        [NonSerialized] public List<CoordinateSystem> CoordinateSystems = new List<CoordinateSystem>();
        
        [SerializeField] protected WeaponParams _weaponParamsLocal;
        public GenomeStats GenomeStats;
        
        public Transform ProjectilesParentTransform;
        public Transform ProjectileSpawnPoint;
        // call this function in Awake() in derived class
        protected void OnAwakeFunc() {
            if (ProjectilesParentTransform == null)
                ProjectilesParentTransform = GameObject.Find("TemporalObjects").transform;
            
            ProjectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
           
            _weaponSO.UpdateParamsEvent += InitializeParams;
            _weaponSO.LaunchCoordinateSystemsEvent += LaunchCoordinateSystems;
            _weaponSO.EnableCoroutinesEvent += EnableCoroutines;
        }
        
        // call this function in OnDestroy() in derived class
        protected void OnDestroyFunc() {
            _weaponSO.UpdateParamsEvent -= InitializeParams;
            _weaponSO.LaunchCoordinateSystemsEvent -= LaunchCoordinateSystems;
            _weaponSO.EnableCoroutinesEvent -= EnableCoroutines;
        }
        
        protected virtual void InitializeParams() {
            _weaponParamsLocal = new WeaponParams(_weaponSO);
            TryToLoadGenomeFromSO();
        }
        
        protected virtual void LaunchCoordinateSystems() {
            foreach (CoordinateSystem system in CoordinateSystems) {
                system.IsMoving = true;
                system.MoveSpeed = _weaponParamsLocal.MoveSpeed;
                system.Direction = ProjectileSpawnPoint.up;
                
                system.IsRotating = true;
                system.RotatingSpeed = _weaponParamsLocal.RotationSpeed;
                system.RotationAcc = _weaponParamsLocal.RotationSpeed * 0.5f;
            }
        }
        
        protected void EnableCoroutines(bool value) {
            StopAllCoroutines();
            
            if (value) 
                FireCoroutine = StartCoroutine(Fire());
        }

       
        
        private void TryToLoadGenomeFromSO() {
            if (_weaponSO.GenomeXml == null) 
                return;
            
            XmlDocument genomeXml = new XmlDocument();
            genomeXml.LoadXml(_weaponSO.GenomeXml.text);
            
            List<NeatGenome> genomeList = NeatGenomeXmlIO.LoadCompleteGenomeList(genomeXml, true, EvolutionAlgorithm.Instance.CppnGenomeFactory);
            GenomeStats = new GenomeStats(genomeList[0], EvolutionAlgorithm.Instance.Decoder, EvolutionAlgorithm.Instance.CppnGenomeFactory);
        }
        
        
        public Coroutine FireCoroutine;
        /// <summary>
        /// To start coroutine => FireCoroutine = StartCoroutine(Fire()); <br />
        /// To stop coroutine => StopCoroutine(FireCoroutine);
        /// </summary>
        public virtual IEnumerator Fire() {
            while (true) {
                switch (_weaponParamsLocal.WeaponMode) {

                    case WeaponMode.MultiShot:
                        FireMultiShot();
                        break;
                    
                    case WeaponMode.Burst:
                        // Wait for FireBurst Coroutine to complete
                        yield return FireBurst();
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                yield return new WaitForSeconds(_weaponParamsLocal.FireRate);
            }
        }
        
        public virtual void FireMultiShot() {
            
            CoordinateSystem localCoordinateSystem = CreateLocalCoordinateSystem();
            
            for (int i = 0; i < _weaponParamsLocal.ProjectilesInOneShot; i++) {
                // Offset for InitialFlight
                float offset = 0;
                if (_weaponParamsLocal.ProjectilesInOneShot != 1)
                    offset = Mathf.Lerp(-1f, 1f, (float)i / (_weaponParamsLocal.ProjectilesInOneShot - 1));

                Projectile projectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity, localCoordinateSystem.transform).GetComponent<Projectile>();
                
                // initialize projectile with necessary data
                projectile.OriginTransform = localCoordinateSystem.transform;
                projectile.Box = GenomeStats.Box;

                projectile.WeaponParamsLocal = new WeaponParams(_weaponParamsLocal);
                projectile.WeaponSo = _weaponSO;
                    
                projectile.SignX = (offset < 0 ? -1f : 1f) * _weaponParamsLocal.SignX;
                projectile.SignY = _weaponParamsLocal.SignY;

                Vector2 initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * offset) * ProjectileSpawnPoint.up;
                projectile.InitialVelocity = initialDirection.normalized * _weaponParamsLocal.InitialSpeed;
            }
        }
        
        
        public virtual IEnumerator FireBurst() {
            int projectileCount = _weaponParamsLocal.ProjectilesInOneShot;
            BurstMode burstMode = _weaponParamsLocal.BurstMode;
            
            List<float> offsets = new List<float>(projectileCount);
            if (projectileCount == 1)
                offsets.Add(0f);
            else {
                for (int i = 0; i < projectileCount; i++) 
                    offsets.Add(Mathf.Lerp(-1f, 1f, (float)i / (projectileCount - 1)));
            }
            
            int negIdx = projectileCount / 2 - 1; 
            int posIdx = projectileCount / 2;
            for (int i = 0; i < projectileCount; i++) {
                
                CoordinateSystem localCoordinateSystem = CreateLocalCoordinateSystem();
                Projectile projectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity, localCoordinateSystem.transform).GetComponent<Projectile>();

                // initialize projectile with necessary data
                projectile.OriginTransform = localCoordinateSystem.transform;
                projectile.Box = GenomeStats.Box;

                projectile.WeaponParamsLocal = new WeaponParams(_weaponParamsLocal);
                projectile.WeaponSo = _weaponSO;

                float offset;
                Vector2 initialDirection;
                switch (burstMode) {

                    case BurstMode.Clockwise:
                        offset = offsets[projectileCount - 1 - i];
                        initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * offset) * ProjectileSpawnPoint.up;
                        break;
                    
                    case BurstMode.CounterClockwise:
                        offset = offsets[i];
                        initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * offset) * ProjectileSpawnPoint.up;
                        break;
                    
                    case BurstMode.Alternate:
                        if (i % 2 == 0) {
                            offset = offsets[posIdx];
                            posIdx++;
                        }
                        else {
                            offset = offsets[negIdx];
                            negIdx--;
                        }
                        initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * offset) * ProjectileSpawnPoint.up;
                        break;
                    
                    case BurstMode.Straight:
                        if (i % 2 == 0) {
                            offset = offsets[posIdx];
                            posIdx++;
                        }
                        else {
                            offset = offsets[negIdx];
                            negIdx--;
                        }
                        initialDirection = ProjectileSpawnPoint.up;
                        break;
                    
                    case BurstMode.MaxMinAngle:
                        if (i % 2 == 0) {
                            offset = offsets[posIdx];
                            posIdx++;
                        }
                        else {
                            offset = offsets[negIdx];
                            negIdx--;
                        }
                        initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * (offset < 0 ? -1f : 1f)) * ProjectileSpawnPoint.up;
                        break;

                    case BurstMode.Random:
                        offset = offsets[Random.Range(0, projectileCount)];
                        initialDirection = Quaternion.Euler(0, 0, _weaponParamsLocal.Angle * offset) * ProjectileSpawnPoint.up;
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                projectile.SignY = _weaponParamsLocal.SignY;
                projectile.SignX = (offset < 0 ? -1f : 1f) * _weaponParamsLocal.SignX;
                projectile.InitialVelocity = initialDirection.normalized * _weaponParamsLocal.InitialSpeed;
                
                yield return new WaitForSeconds(_weaponParamsLocal.BurstRate);
            }
        }
        
        private CoordinateSystem CreateLocalCoordinateSystem() {
            // Projectiles instantiated in the same multishot will use their localCoordinateSystem to calculate their local coordinates. 
            CoordinateSystem localCoordinateSystem = Instantiate(CoordinateSystemPrefab, ProjectilesParentTransform);
            localCoordinateSystem.Weapon = this;
            
            // localCoordinateSystem has the same rotation and position as ProjectileSpawnPoint in the moment of the shot
            localCoordinateSystem.transform.up = ProjectileSpawnPoint.up;            
            localCoordinateSystem.transform.right = ProjectileSpawnPoint.right;      
            localCoordinateSystem.transform.rotation = ProjectileSpawnPoint.rotation;
            localCoordinateSystem.transform.position = ProjectileSpawnPoint.position;
            
            CoordinateSystems.Add(localCoordinateSystem);
            return localCoordinateSystem;
        }
        
        
        
        
        
        // Visualization of initial flight circle radius, reflection borders and network control distance
        private float _borderRayDirX, _borderRayDirY;
        private Vector2 _upperBorderRayDir, _lowerBorderRayDir;
        private void OnDrawGizmosSelected() {
            if (ProjectileSpawnPoint == null)
                return;
            
            switch (_weaponParamsLocal.Mode) {

                case ReflectionMode.CircleReflection:
                    Gizmos.DrawWireSphere(ProjectileSpawnPoint.position, _weaponParamsLocal.NNControlDistance * _weaponParamsLocal.InitialFlightRadius);
                    Gizmos.DrawWireSphere(ProjectileSpawnPoint.position, _weaponParamsLocal.NNControlDistance * Mathf.Sqrt(2));
                    Gizmos.DrawWireSphere(ProjectileSpawnPoint.position, _weaponParamsLocal.NNControlDistance * _weaponParamsLocal.ReflectiveCircleRadius);
                    break;
                
                case ReflectionMode.Polar:
                    _borderRayDirX = _weaponParamsLocal.NNControlDistance * Mathf.Sin(_weaponParamsLocal.MaxPolarAngleDeg * Mathf.Deg2Rad);
                    _borderRayDirY = _weaponParamsLocal.NNControlDistance * Mathf.Cos(_weaponParamsLocal.MaxPolarAngleDeg * Mathf.Deg2Rad);
                    
                    _upperBorderRayDir= ProjectileSpawnPoint.transform.TransformVector(new Vector2(- _borderRayDirX, _borderRayDirY)).normalized;
                    _lowerBorderRayDir = ProjectileSpawnPoint.transform.TransformVector(new Vector2(_borderRayDirX, _borderRayDirY)).normalized;
                    _upperBorderRayDir *= _weaponParamsLocal.NNControlDistance * Mathf.Sqrt(2);
                    _lowerBorderRayDir *= _weaponParamsLocal.NNControlDistance * Mathf.Sqrt(2);
                    
                    Gizmos.DrawWireSphere(ProjectileSpawnPoint.position, _weaponParamsLocal.NNControlDistance * _weaponParamsLocal.InitialFlightRadius);
                    Gizmos.DrawRay(ProjectileSpawnPoint.position, _upperBorderRayDir);
                    Gizmos.DrawRay(ProjectileSpawnPoint.position, _lowerBorderRayDir);
                    break;
                
                case ReflectionMode.RectangleReflection:
                    float maxX = _weaponParamsLocal.RectDimensions.x * _weaponParamsLocal.NNControlDistance;
                    float maxY = _weaponParamsLocal.RectDimensions.y * _weaponParamsLocal.NNControlDistance;
                    
                    Gizmos.DrawWireCube(ProjectileSpawnPoint.position, new Vector2(maxY * 2f, maxX * 2f));
                    Gizmos.DrawWireSphere(ProjectileSpawnPoint.position, _weaponParamsLocal.NNControlDistance * _weaponParamsLocal.InitialFlightRadius);
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}
