using System;
using System.Globalization;
using System.Linq;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace NeatBullets.WeaponDemo.Scripts {
    public class UIBox : MonoBehaviour {
        
        // assigned from the editor
        [SerializeField] private Button _nextBtn;
        [SerializeField] private Button _prevBtn;
        [SerializeField] private Button _hasTrailBtn;
        [SerializeField] private Button _forceViewBtn;
        [SerializeField] private Button _combinedWeaponsBtn;
        [SerializeField] private Button _launchBtn;
        [SerializeField] private TextMeshProUGUI _weaponText;
        [SerializeField] private DemoWeapon _demoWeapon;
        [SerializeField] private CombinedWeapon _combinedWeapon;
        [SerializeField] private WeaponParamsSO[] _weaponList;
        [SerializeField] private ArrowSpawner _arrowSpawner;
        
        
        private int _counter, _counterCW;
        private int _weaponCount, _weaponCountCW;
        private void Awake() {
            _counter = 0;
            _counterCW = 0;
            _weaponCount = _weaponList.Length;
            _weaponCountCW = _combinedWeapon.WeaponCount;

            foreach (WeaponParamsSO so in _weaponList) {
                so.LoadParamsFromJson();
                so.Brightness = 1f;
                so.Saturation = 1f;
                so.Lifespan = 10f;
            }
            
            _nextBtn.onClick.AddListener(OnNextBtnClick);
            _prevBtn.onClick.AddListener(OnPrevBtnClick);
            
            _hasTrailBtn.onClick.AddListener(OnTrailBtnClick);
            _forceViewBtn.onClick.AddListener(OnForceViewClick);
            _combinedWeaponsBtn.onClick.AddListener(OnCombinedWeaponsClick);
            _launchBtn.onClick.AddListener(OnLaunchClick);
        }

        private void OnDestroy() {
            _nextBtn.onClick.RemoveAllListeners();
            _prevBtn.onClick.RemoveAllListeners();
            
            _hasTrailBtn.onClick.RemoveListener(OnTrailBtnClick);
            _forceViewBtn.onClick.RemoveListener(OnForceViewClick);
            _combinedWeaponsBtn.onClick.RemoveListener(OnCombinedWeaponsClick);
            _launchBtn.onClick.RemoveListener(OnLaunchClick);
        }
        
        private void Start() {
            _arrowSpawner.CreateForceMap();
            UpdateAll();
            InvokeRepeating(nameof(UpdateText), 0.1f, 0.1f);
        }

        private void OnNextBtnClick() {
            if (_counter == _weaponCount - 1)
                _counter = 0;
            else
                _counter++;

            UpdateAll();
        }
        
        private void OnPrevBtnClick() {
            if (_counter == 0)
                _counter = _weaponCount - 1;
            else
                _counter--;
            
            UpdateAll();
        }
        
        private void OnNextBtnClickCW() {
            if (_counterCW == _weaponCountCW - 1)
                _counterCW = 0;
            else
                _counterCW++;
            
            _combinedWeapon.UpdateWeaponSO(_counterCW);
        }
        
        private void OnPrevBtnClickCW() {
            if (_counterCW == 0)
                _counterCW = _weaponCountCW - 1;
            else
                _counterCW--;
            
            _combinedWeapon.UpdateWeaponSO(_counterCW);
        }

        private bool _hasTrail = false;
        private void OnTrailBtnClick() {
            _hasTrail = !_weaponList[_counter].HasTrail;
            foreach (WeaponParamsSO so in _weaponList)
                so.HasTrail = _hasTrail;

            _demoWeapon.UpdateWeaponSO(_weaponList[_counter]);
        }

        private bool _arrowsVisible = false;
        private void OnForceViewClick() {
            if (!_arrowsVisible) {
                ArrowsSpritesEnabled(true);
                _arrowsVisible = true;
            }
            else {
                ArrowsSpritesEnabled(false);
                _arrowsVisible = false;
            }
            
        }

        private void ArrowsSpritesEnabled(bool value) {
            foreach (GameObject go in _arrowSpawner.ArrowList) {
                ForceArrow arrow = go.GetComponent<ForceArrow>();
                foreach (SpriteRenderer sr in arrow.SpriteRenderers) 
                    sr.enabled = arrow.InNNControlZone && value;
                
            }
        }
        
        private bool _isCombinedWeaponsMode = false;
        private void OnCombinedWeaponsClick() {
            _isCombinedWeaponsMode = !_isCombinedWeaponsMode;

            Color disabledTextColor = new Color(137 / 255f, 137 / 255f, 137 / 255f);
            Color defaultTextColor = new Color(230 / 255f, 230 / 255f, 230 / 255f);
            
            // disable/enable buttons depending on mode
            _hasTrailBtn.interactable = !_isCombinedWeaponsMode;
            _forceViewBtn.interactable = !_isCombinedWeaponsMode;
            _launchBtn.interactable = !_isCombinedWeaponsMode;
            _hasTrailBtn.GetComponentInChildren<TextMeshProUGUI>().color = _isCombinedWeaponsMode ? disabledTextColor : defaultTextColor;
            _forceViewBtn.GetComponentInChildren<TextMeshProUGUI>().color = _isCombinedWeaponsMode ? disabledTextColor : defaultTextColor;
            _launchBtn.GetComponentInChildren<TextMeshProUGUI>().color = _isCombinedWeaponsMode ? disabledTextColor : defaultTextColor;
            
            // change functionality of Next and Prev buttons depending on mode
            if (_isCombinedWeaponsMode) {
                _nextBtn.onClick.AddListener(OnNextBtnClickCW);
                _prevBtn.onClick.AddListener(OnPrevBtnClickCW);
                
                _nextBtn.onClick.RemoveListener(OnNextBtnClick);
                _prevBtn.onClick.RemoveListener(OnPrevBtnClick);
            }
            else {
                _nextBtn.onClick.AddListener(OnNextBtnClick);
                _prevBtn.onClick.AddListener(OnPrevBtnClick);
                
                _nextBtn.onClick.RemoveListener(OnNextBtnClickCW);
                _prevBtn.onClick.RemoveListener(OnPrevBtnClickCW);
            }
            
            _demoWeapon.GetComponentInChildren<SpriteRenderer>().enabled = !_isCombinedWeaponsMode;
            _combinedWeapon.GetComponentInChildren<SpriteRenderer>().enabled = _isCombinedWeaponsMode;
            
            if (_isCombinedWeaponsMode) {
                _demoWeapon.StopAllCoroutines();
                _weaponList[_counter].DestroyProjectilesEvent?.Invoke();
                
                ArrowsSpritesEnabled(false);
                _arrowsVisible = false;
                
                _combinedWeapon.UpdateWeaponSO(_counterCW);
            }
            else {
                _combinedWeapon.StopAllCoroutines();
                _combinedWeapon.WeaponSoArr[_counterCW].DestroyProjectilesEvent?.Invoke();
                
                UpdateAll();
            }
            
            
        }

        private void OnLaunchClick() {
            _demoWeapon.LaunchForward();
        }

        private void UpdateAll() {
            UpdateText();
            _demoWeapon.UpdateWeaponSO(_weaponList[_counter]);
            UpdateArrows(_weaponList[_counter]);
        }
        
        private void UpdateArrows(WeaponParamsSO weaponSO) {
            foreach (GameObject go in _arrowSpawner.ArrowList) { 
                ForceArrow arrow = go.GetComponent<ForceArrow>();

                arrow.WeaponParamsLocal = new WeaponParams(weaponSO);
                arrow.OriginTransform = _demoWeapon.ProjectileSpawnPoint;

                arrow.SignX = weaponSO.SignX;
                arrow.SignY = weaponSO.SignY;

                arrow.Box = _demoWeapon.GenomeStats.Box;
                arrow.InputArr = _demoWeapon.GenomeStats.Box.InputSignalArray;
                arrow.OutputArr = _demoWeapon.GenomeStats.Box.OutputSignalArray;

                arrow.CalcProjectileStats();
                switch (weaponSO.Mode) {
                    case ReflectionMode.CircleReflection:
                        arrow.ActivateBlackBoxCircle();
                        break;
                    case ReflectionMode.RectangleReflection:
                        arrow.ActivateBlackBoxRect();
                        break;
                    case ReflectionMode.Polar:
                        arrow.ActivateBlackBoxPolar();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                arrow.ReadDataFromBlackBox();
            }
            
            if (_arrowsVisible)
                // Enable all arrows, except those not in NNControl zone
                ArrowsSpritesEnabled(true);
        }


        private void UpdateText() {
            if (!_isCombinedWeaponsMode) {

                string weaponMode = _weaponList[_counter].WeaponMode.ToString();
                string reflectionMode = _weaponList[_counter].Mode.ToString();
                string readMode = _weaponList[_counter].ReadMode.ToString();
                string burstMode = _weaponList[_counter].WeaponMode == WeaponMode.Burst
                    ? _weaponList[_counter].BurstMode.ToString()
                    : "";

                int projectilesCount = _demoWeapon.CoordinateSystems.Sum(coordinateSystem => coordinateSystem.transform.childCount);
                string fpsCount = Mathf.RoundToInt(_fps).ToString();

                _weaponText.text = $"{_weaponList[_counter].name}\n" +
                                   $"{weaponMode}{burstMode}\n{reflectionMode}\n{readMode}\n" +
                                   $"#{_counter}\n" +
                                   $"Projectiles: {projectilesCount.ToString()}\n" +
                                   $"FPS: {fpsCount}";
                
            }
            else {
                
                int projectilesCount = _combinedWeapon.CoordinateSystems.Sum(coordinateSystem => coordinateSystem.transform.childCount);
                string fpsCount = Mathf.RoundToInt(_fps).ToString();

                _weaponText.text = $"Based on {_combinedWeapon.WeaponName}\n" +
                                   $"CW #{_counterCW}\n" +
                                   $"Projectiles: {projectilesCount.ToString()}\n" +
                                   $"FPS: {fpsCount}";
                
            }
            
        }
        
        
        
        
        
        
        private float _deltaTime, _fps;
        private void Update() {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            _fps = 1.0f / _deltaTime;
        }


    }
}
