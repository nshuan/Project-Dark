using System;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepMoveTowers : UIAbstractTutorialStepInGame
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] protected GameObject objInstruction;
        [SerializeField] protected GameObject objCursorInstruction;
        
        [Space] [Header("Config")]
        [SerializeField] private int levelToShow;
        [SerializeField] private int waveToShow;
        [SerializeField] private int gateToShow;
        
        private int towerToInstruct;
        
        public override bool IsValid()
        {
            if (LevelManager.Instance.Level.level == levelToShow && LevelManager.Instance.CurrentWaveIndex <= waveToShow) return true;
            return false;
        }

        public override void Setup()
        {
            CombatActions.OnGateActivated += OnGateActivated;
            CombatActions.OnTowerHoverIn += OnTowerHoverIn;
            CombatActions.OnTowerHoverOut += OnTowerHoverOut;
        }

        private void OnGateActivated(GateEntity gate, int waveIndex, int gateIndex)
        {
            // Chưa đến wave 1 thì đợi tiếp
            if (waveIndex < waveToShow) return;
            // Đến wave 1 rồi thì check gate
            if (waveIndex == waveToShow)
            {
                // Chưa đến gate 2 thì đợi tiếp
                if (gateIndex < gateToShow) return;
                // Đến gate 2 rồi thì check tower
                if (gateIndex == gateToShow)
                {
                    towerToInstruct = gate.target[0].Id;
            
                    // Đang ở tower cần di chuyển thì complete luôn
                    if (LevelManager.Instance.CurrentTower.Id == towerToInstruct)
                    {
                        objCursorInstruction.SetActive(false);
                        CombatActions.OnGateActivated -= OnGateActivated;
                        CombatActions.OnTowerHoverIn -= OnTowerHoverIn;
                        CombatActions.OnTowerHoverOut -= OnTowerHoverOut;
                        OnComplete?.Invoke();
                        return;
                    }
                
                    // Còn không thì show instruction và đợi move sang tower cần di chuyển
                    CombatActions.OnGateActivated -= OnGateActivated;
                    CombatActions.OnTowerHoverIn -= OnTowerHoverIn;
                    CombatActions.OnTowerHoverOut -= OnTowerHoverOut;
                    if (mainCamera) objInstruction.transform.position = mainCamera.WorldToScreenPoint(gate.target[0].transform.position);
                    objCursorInstruction.SetActive(false);
                    objInstruction.SetActive(true);
                    actionUpdateFocus?.Invoke(
                        objInstruction.transform.localPosition,
                        new Vector2(0.06f, 0.1f),
                        2f,
                        false,
                        false);
                    LevelManager.Instance.OnChangeTower += OnTowerChanged;
                    return;
                }
            }
            
            // Đã quá wave 1 gate 2 rồi thì complete luôn
            OnComplete?.Invoke();
        }

        private void OnTowerChanged(TowerEntity tower)
        {
            if (tower.Id == towerToInstruct)
            {
                objCursorInstruction.SetActive(false);
                LevelManager.Instance.OnChangeTower -= OnTowerChanged;
                CombatActions.OnTowerHoverIn -= OnTowerHoverIn;
                CombatActions.OnTowerHoverOut -= OnTowerHoverOut;
                OnComplete?.Invoke();
            }
        }

        private void OnTowerHoverIn(TowerEntity tower)
        {
            objCursorInstruction.SetActive(true);
        }

        private void OnTowerHoverOut(TowerEntity tower)
        {
            objCursorInstruction.SetActive(false);
        }

        private void Update()
        {
            if (objCursorInstruction.activeInHierarchy)
                objCursorInstruction.transform.position = Input.mousePosition;

            if (objInstruction.activeInHierarchy)
            {
                actionUpdateFocus?.Invoke(
                    objInstruction.transform.localPosition,
                    new Vector2(0.06f, 0.1f),
                    2f,
                    false,
                    false);
            }
        }
    }
}