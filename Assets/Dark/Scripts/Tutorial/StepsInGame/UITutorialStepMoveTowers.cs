using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepMoveTowers : UIAbstractTutorialStepInGame
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] protected GameObject objInstruction;
        
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
                        OnComplete?.Invoke();
                        return;
                    }
                
                    // Còn không thì show instruction và đợi move sang tower cần di chuyển
                    CombatActions.OnGateActivated -= OnGateActivated;
                    if (mainCamera) objInstruction.transform.position = mainCamera.WorldToScreenPoint(gate.target[0].transform.position);
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
                LevelManager.Instance.OnChangeTower -= OnTowerChanged;
                OnComplete?.Invoke();
            }
        }
    }
}