using System;
using System.Collections.Generic;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using NeatBullets.Core.Scripts.WeaponSystem.NEAT;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using UnityEngine.UI;
using GameWeapon = NeatBullets.ShadowSurvival.Scripts.Player.GameWeapon;
using Random = UnityEngine.Random;

namespace NeatBullets.ShadowSurvival.Scripts.EvolutionSystem {
    public class WeaponItemSpawner : MonoBehaviour {
        
        // Assign in the editor
        [SerializeField] private WeaponItem _weaponItemPrefab;
        [SerializeField] private WeaponParamsSO _weaponItemSo;
        
        private Camera _camera;
        private Player.Player _player;
        private void Awake() {
            _camera = Camera.main;
            _player = FindObjectOfType<Player.Player>();
        }
        
        private int _numberOfItems;
        private IList<NeatGenome> _genomeList;
        private void Start() {
            _numberOfItems = EvolutionAlgorithm.Instance.PopulationSize;
            _genomeList = EvolutionAlgorithm.Instance.GenomeList;
        
            InstantiateWeaponItems();
            InitializeWeaponItems();
        }
        
        
        
        // Assign in the editor
        [SerializeField] private GameObject _pickUpCanvas;
        [SerializeField] private GameObject _joysticksCanvas;
        [SerializeField] private CanvasWeapon _canvasWeapon;
        [SerializeField] private GameWeapon _gameWeapon;
        
        [SerializeField] private Slider _distanceSlider;
        [SerializeField] private Toggle _flip;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _dismissButton;
        
        [SerializeField] private List<WeaponItem> _weaponItems;
        private void InstantiateWeaponItems() {
            for (int i = 0; i < _numberOfItems; i++) {
                WeaponItem item = Instantiate(_weaponItemPrefab, transform);
                item.WeaponItemSpawner = this;
                item.Id = i;
                item.PickUpCanvas = _pickUpCanvas;
                item.JoysticksCanvas = _joysticksCanvas;
                item.CanvasWeapon = _canvasWeapon;
                item.GameWeapon = _gameWeapon;
                
                item.DistanceSlider = _distanceSlider;
                item.Flip = _flip;
                item.AcceptButton = _acceptButton;
                item.DismissButton = _dismissButton;
            
                _weaponItems.Add(item);
            }
        }
        
        private void InitializeWeaponItems() {
            _weaponItemSo.DestroyProjectilesEvent?.Invoke();
            
            for (int i = 0; i < _numberOfItems; i++) {
                RepositionItem(i);
                WeaponItem item = _weaponItems[i];
                
                item.GenomeStats = new GenomeStats(_genomeList[i], 
                            EvolutionAlgorithm.Instance.Decoder,
                            EvolutionAlgorithm.Instance.CppnGenomeFactory);
                
            }
        }

        public void UpdateWeaponItems(int itemId) {
            _weaponItemSo.DestroyProjectilesEvent?.Invoke();
            _weaponItemSo.LoadParamsFromJson();
            
            // override values of WP with random ones
            _weaponItemSo.ForwardForce = false;
            Array wmValues = Enum.GetValues(typeof(WeaponMode));
            _weaponItemSo.WeaponMode = (WeaponMode)wmValues.GetValue(Random.Range(0, wmValues.Length));
            switch (_weaponItemSo.WeaponMode) {
                
                case WeaponMode.MultiShot:
                    _weaponItemSo.FireRate = Random.Range(0.3f, 0.5f);
                    _weaponItemSo.ProjectilesInOneShot = Random.Range(5, 9);
                    break;
                
                case WeaponMode.Burst:
                    Array bmValues = Enum.GetValues(typeof(BurstMode));
                    _weaponItemSo.BurstMode = (BurstMode)bmValues.GetValue(Random.Range(0, bmValues.Length));
                    _weaponItemSo.BurstRate = Random.Range(0.02f, 0.1f);
                    _weaponItemSo.ProjectilesInOneShot = Random.Range(20, 30);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _weaponItemSo.MaxPolarAngleDeg = Random.Range(8f, 40f);
            _weaponItemSo.Angle = Random.Range(4f, _weaponItemSo.MaxPolarAngleDeg - 2f);
            _weaponItemSo.InitialFlightRadius = Random.Range(0.1f, 0.4f);
            _weaponItemSo.InitialSpeed = Random.Range(4f, 5f);
            
            float minSpeed = Random.Range(4.5f, 7f);
            float maxSpeed = Random.Range(minSpeed, 8f);
            _weaponItemSo.SpeedRange = new Vector2(minSpeed, maxSpeed);
            
            float minHue = Random.Range(0f, 0.99f);
            float maxHue = Random.Range(minHue, 1f);
            _weaponItemSo.HueRange = new Vector2(minHue, maxHue);
            
            // ignore output x with 0.5 probability
            _weaponItemSo.SignX = Random.value > 0.5f ? 0f : 1f;
            
            RepositionItem(itemId);
            for (int i = 0; i < _numberOfItems; i++) {
                WeaponItem item = _weaponItems[i];
                
                item.GenomeStats = new GenomeStats(_genomeList[i], 
                    EvolutionAlgorithm.Instance.Decoder,
                    EvolutionAlgorithm.Instance.CppnGenomeFactory);
                
            }
        }
        
        
        [Header("Item spawn")] 
        [SerializeField] [Range(1f, 40f)] private float _xMargin = 20f;
        [SerializeField] [Range(1f, 40f)] private float _yMargin = 20f;
        private void RepositionItem(int i) {
            Vector2 p11 = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            Vector2 p00 = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            
            // Rectangle with camera view hole inside, where items cannot spawn
            List<Vector3> posList = new List<Vector3>();
            Vector3 posOnLeftBand = new Vector3(Random.Range(p00.x - _xMargin, p00.x),
                Random.Range(p00.y - _yMargin, p11.y + _yMargin), 0);
            Vector3 posOnRightBand = new Vector3(Random.Range(p11.x, p11.x + _xMargin),
                Random.Range(p00.y - _yMargin, p11.y + _yMargin), 0);
            Vector3 posOnUpperBand = new Vector3(Random.Range(p00.x - _xMargin, p11.x + _xMargin),
                Random.Range(p11.y, p11.y + _yMargin), 0);
            Vector3 posOnLowerBand = new Vector3(Random.Range(p00.x - _xMargin, p11.x + _xMargin),
                Random.Range(p00.y - _yMargin, p00.y), 0);
            posList.AddRange(new[] { posOnLeftBand, posOnRightBand, posOnUpperBand, posOnLowerBand });
            
            _weaponItems[i].transform.position = posList[Random.Range(0, posList.Count)];
        }



        private void LateUpdate() {
             foreach (WeaponItem item in _weaponItems) {
                 float distanceToPlayer = Vector2.Distance(item.transform.position, _player.transform.position);
                 if (distanceToPlayer > 50f) 
                     RepositionItem(item.Id);
             }
        }


        // Visualize spawn zone
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
    
    
    }
}
