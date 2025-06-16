using System.Collections;
using System.Collections.Generic;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using NeatBullets.SpaceShooter.Scripts.EnemyBehaviour;
using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.Managers {
    public class EnemySpawner : MonoBehaviour
    {
        // Assign in editor
        [SerializeField] private GlobalVariablesSO _globalVariables;
        [SerializeField] private List<Sprite> _enemySprites;
        [SerializeField] private WeaponParamsSO[] _weaponSoList;
        [SerializeField] private GameObject _enemyPrefab;
        
        [HideInInspector] [SerializeField] private Camera _camera;
        private void Awake() {
            _camera = Camera.main;
            
            foreach (WeaponParamsSO weaponParams in _weaponSoList) {
                // Reset SO values to json's
                weaponParams.LoadParamsFromJson();
                
                // Override values
                weaponParams.Lifespan = 4f;
                if (weaponParams.ProjectilesInOneShot > 6)
                    weaponParams.ProjectilesInOneShot = 6;
                
                weaponParams.Mode = ReflectionMode.Polar;
                weaponParams.InitialSpeed = Random.Range(3f, 5f);
                weaponParams.MaxPolarAngleDeg = Random.Range(5f, 30f);
                weaponParams.ForwardForce = true;
                weaponParams.Angle = Random.Range(2f, weaponParams.MaxPolarAngleDeg);
                weaponParams.NNControlDistance = 8f;
                weaponParams.HasTrail = true;
            }
        }
        
        
        private Coroutine _spawnEnemiesCoroutine;

        [Header("Enemy spawn")] 
        [SerializeField] [Range(1f, 5f)] private float _xMargin = 1f;
        [SerializeField] [Range(1f, 5f)] private float _yMargin = 1f;
        [SerializeField] [Range(2f, 10f)] private float _spawnRate = 4f;
        private IEnumerator SpawnEnemies() {
            while (true) {
                yield return new WaitForSeconds(_spawnRate * 0.5f);
                Vector2 p11 = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
                Vector2 p00 = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
                
                // Rectangle with camera view hole inside, where enemies cannot spawn
                List<Vector3> posList = new List<Vector3>();
                Vector3 posOnLeftBand = new Vector3(Random.Range(p00.x - _xMargin, p00.x), Random.Range(p00.y - _yMargin, p11.y + _yMargin), 0);
                Vector3 posOnRightBand = new Vector3(Random.Range(p11.x, p11.x + _xMargin), Random.Range(p00.y - _yMargin, p11.y + _yMargin), 0);
                Vector3 posOnUpperBand = new Vector3(Random.Range(p00.x - _xMargin, p11.x + _xMargin), Random.Range(p11.y, p11.y + _yMargin), 0);
                Vector3 posOnLowerBand = new Vector3(Random.Range(p00.x - _xMargin, p11.x + _xMargin), Random.Range(p00.y - _yMargin, p00.y), 0);
                posList.AddRange(new []{posOnLeftBand, posOnRightBand, posOnUpperBand, posOnLowerBand});
                
                EnemyController enemy = Instantiate(_enemyPrefab, posList[Random.Range(0, posList.Count)], Quaternion.identity, transform).
                    GetComponent<EnemyController>();
                
                // Transfer SOs and randomize stats
                enemy.Sprite = _enemySprites[Random.Range(0, _enemySprites.Count)];
                enemy.Weapon.UpdateWeaponSO(_weaponSoList[Random.Range(0, _weaponSoList.Length)]);
                enemy.HealthPoints = Random.Range(10f, 30f);
                enemy.MovementSpeed = Random.Range(3f, 4f);
                enemy.AttackDistance = Random.Range(6f, 8f);
                
                yield return new WaitForSeconds(_spawnRate * 0.5f);
            }
        }

        // Visualize enemy spawn zone
        private void OnDrawGizmos() {
            if (_camera == null)
                return;
            
            Vector2 p11 = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            Vector2 p00 = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            
            Vector3[] points = new Vector3[4] {
                // lowerLeftPoint
                new Vector3(p00.x - _xMargin, p00.y - _yMargin, 0), 
                // lowerRightPoint
                new Vector3(p11.x + _xMargin, p00.y - _yMargin, 0),
                // upperRightPoint
                new Vector3(p11.x + _xMargin, p11.y + _yMargin, 0),
                // upperLeftPoint
                new Vector3(p00.x - _xMargin, p11.y + _yMargin, 0)
            };
            
            Gizmos.DrawLineStrip(points, true);
        }

        
        private void Start() {
            _spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
            _globalVariables.OnPauseResumeEvent += OnPauseResumeGame;
        }

        private void OnDestroy() {
            _globalVariables.OnPauseResumeEvent -= OnPauseResumeGame;
        }
        
        private void OnPauseResumeGame() {
            if (_globalVariables.IsPaused)
                StopCoroutine(_spawnEnemiesCoroutine);
            else
                _spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
        }

        
        

       
        
    }
}
