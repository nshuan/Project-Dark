using System;
using System.Collections.Generic;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem.NEAT;
using SharpNeat.Genomes.Neat;
using Unity.Mathematics;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem {
    public class WeaponManager : MonoBehaviour
    {
        // assign in the editor
        [SerializeField] private GameObject _weaponPrefab;
        [SerializeField] private WeaponParamsSO _weaponSo;
        
        private Camera _camera;
        private void Awake() {
            _camera = Camera.main;
        }
    
        private EvolutionAlgorithm _evolutionAlgorithm;
        private int _numberOfWeapons;
        private IList<NeatGenome> _genomeList;
        private List<GameObject> _weapons = new List<GameObject>();
        private void Start() {
            _evolutionAlgorithm = EvolutionAlgorithm.Instance;
            _numberOfWeapons = _evolutionAlgorithm.PopulationSize;
            _genomeList = _evolutionAlgorithm.GenomeList;
        
            InstantiateWeapons();
            InitializeWeapons();
        
            _evolutionAlgorithm.NewGenEvent += InitializeWeapons;
        }

        private void OnDestroy() {
            _evolutionAlgorithm.NewGenEvent -= InitializeWeapons;
        }
        
        private void InstantiateWeapons() {
            for (int i = 0; i < _numberOfWeapons; i++) 
                _weapons.Add(Instantiate(_weaponPrefab, transform));
        }
        
        private void InitializeWeapons() {
            RepositionWeapons();
            _weaponSo.DestroyProjectilesEvent?.Invoke();
            for (int i = 0; i < _numberOfWeapons; i++) {
                EvoWeapon weapon = _weapons[i].GetComponent<EvoWeapon>();
                
                weapon.StopAllCoroutines();
                
                weapon.GenomeStats = new GenomeStats(_genomeList[i], _evolutionAlgorithm.Decoder, _evolutionAlgorithm.CppnGenomeFactory);
                
                weapon.FireCoroutine = weapon.StartCoroutine(weapon.Fire());
            }
        }

        
        private void RepositionWeapons() {
            Vector2 p11 = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            Vector2 p00 = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));

            float horizontalSpacing = (p11.x - p00.x) / (_numberOfWeapons / 2 + 1);
            float marginY = (p11.y - p00.y) * .25f;
            
            
            if (_weaponSo.Mode == ReflectionMode.Polar) {
                
                for (int j = 0; j < _numberOfWeapons / 2; j++) {
                    Vector3 pos = new Vector3(p00.x + j * horizontalSpacing + horizontalSpacing * 1.5f, p11.y - marginY * 2.25f, 0);
                    _weapons[j].transform.position = pos;
                    _weapons[j].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
            
                for (int j = 0; j <_numberOfWeapons - _numberOfWeapons / 2; j++) {
                    Vector3 pos = new Vector3(p00.x + j * horizontalSpacing + horizontalSpacing, p00.y + marginY * 0.75f, 0);
                    _weapons[j + _numberOfWeapons / 2].transform.position = pos;
                    _weapons[j + _numberOfWeapons / 2].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
                
            } else {
                
                for (int j = 0; j < _numberOfWeapons / 2; j++) {
                    Vector3 pos = new Vector3(p00.x + j * horizontalSpacing + horizontalSpacing * 1.25f, p11.y - marginY * 1.1f, 0);
                    _weapons[j].transform.position = pos;
                    _weapons[j].transform.rotation = Quaternion.identity;
                }
            
                for (int j = 0; j <_numberOfWeapons - _numberOfWeapons / 2; j++) {
                    Vector3 pos = new Vector3(p00.x + j * horizontalSpacing + horizontalSpacing * 0.75f, p00.y + marginY * 1.1f, 0);
                    _weapons[j + _numberOfWeapons / 2].transform.position = pos;
                    _weapons[j + _numberOfWeapons / 2].transform.rotation = Quaternion.identity;
                }
                
            }
            
            
            
            
            
        }
        
    }
}
