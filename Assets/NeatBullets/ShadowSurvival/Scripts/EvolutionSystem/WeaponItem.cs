using NeatBullets.Core.Scripts.WeaponSystem;
using NeatBullets.Core.Scripts.WeaponSystem.NEAT;
using NeatBullets.ShadowSurvival.Scripts.Player;
using NeatBullets.SpaceShooter.Scripts.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NeatBullets.ShadowSurvival.Scripts.EvolutionSystem {
    public class WeaponItem : AbstractWeapon {
        
        //[Header("Weapon Item Section")] 
        
        // Assigned by WeaponItemSpawner
        [HideInInspector] public WeaponItemSpawner WeaponItemSpawner;
        [HideInInspector] public int Id;
        [HideInInspector] public GameObject PickUpCanvas;
        [HideInInspector] public GameObject JoysticksCanvas;
        [HideInInspector] public CanvasWeapon CanvasWeapon;
        [HideInInspector] public GameWeapon GameWeapon;
        
        [HideInInspector] public Slider DistanceSlider;
        [HideInInspector] public Toggle Flip;
        [HideInInspector] public Button AcceptButton;
        [HideInInspector] public Button DismissButton;

        
        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player"))
                return;
            
            
            ActivatePickUpCanvas();
            JoysticksCanvas.SetActive(false);
            GameManager.Instance.Pause(true);
        }
        
        private void ActivatePickUpCanvas() {
            PickUpCanvas.SetActive(true);
            
            CanvasWeapon.GenomeStats = base.GenomeStats;
            DistanceSlider.value = base._weaponSO.NNControlDistance;
            Flip.isOn = false;

            CanvasWeapon.Initialize();
            CanvasWeapon.FireCoroutine = CanvasWeapon.StartCoroutine(CanvasWeapon.Fire());
            
            
            DistanceSlider.onValueChanged.AddListener(OnSliderChanged);
            Flip.onValueChanged.AddListener(OnFlipChanged);
            AcceptButton.onClick.AddListener(OnAcceptWeapon);
            DismissButton.onClick.AddListener(OnDismissWeapon);
        }
        
        private void DisablePickUpCanvas() {
            PickUpCanvas.SetActive(false);
            
            CanvasWeapon.StopAllCoroutines();
            
            EvolutionAlgorithm.Instance.CreateRandomPopulation();
            WeaponItemSpawner.UpdateWeaponItems(Id);
            
            
            DistanceSlider.onValueChanged.RemoveListener(OnSliderChanged);
            Flip.onValueChanged.RemoveListener(OnFlipChanged);
            AcceptButton.onClick.RemoveListener(OnAcceptWeapon);
            DismissButton.onClick.RemoveListener(OnDismissWeapon);
        }
        
        private void OnAcceptWeapon() {
            GameWeapon.GenomeStats = base.GenomeStats;
            GameWeapon.UpdateWeaponSO(_weaponSO);
            DisablePickUpCanvas();
        
            JoysticksCanvas.SetActive(true);
            GameManager.Instance.Pause(false);
        }
        
        private void OnDismissWeapon() {
            DisablePickUpCanvas();
        
            JoysticksCanvas.SetActive(true);
            GameManager.Instance.Pause(false);
        }
        
        private void OnSliderChanged(float distance) {
            _weaponSO.NNControlDistance = distance;
            _weaponSO.UpdateParamsEvent.Invoke();
            
            CanvasWeapon.RestartCoroutine();
        }
        
        
        private void OnFlipChanged(bool value) {
            _weaponSO.SignY *= -1f;
            _weaponSO.UpdateParamsEvent.Invoke();
            
            CanvasWeapon.RestartCoroutine();
        }

        
        
    }
}
