using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepMoveTowers : UIAbstractTutorialStepInGame
    {
        [SerializeField] protected GameObject objInstruction;
        
        [Space] [Header("Config")]
        [SerializeField] private int levelToShow;
        [SerializeField] private int waveToShow;
        [SerializeField] private int towerToInstruct;
        
        public override bool IsValid()
        {
            if (LevelManager.Instance.Level.level == levelToShow && LevelManager.Instance.CurrentWaveIndex < waveToShow) return true;
            return false;
        }

        public override void Setup()
        {
            LevelManager.Instance.OnWaveStart += OnWaveStart;
        }

        private void OnWaveStart(int waveIndex, float timeToEnd)
        {
            // Chưa đến wave 1 thì đợi tiếp
            if (waveIndex < waveToShow) return;
            // Đến wave 1 rồi thì check tower
            if (waveIndex == waveToShow)
            {
                // Đang ở tower cần di chuyển thì complete luôn
                if (LevelManager.Instance.CurrentTower.Id == towerToInstruct)
                {
                    OnComplete?.Invoke();
                    return;
                }
                
                // Còn không thì show instruction và đợi move sang tower cần di chuyển
                LevelManager.Instance.OnWaveStart -= OnWaveStart;
                objInstruction.SetActive(true);
                actionUpdateFocus?.Invoke(
                    objInstruction.transform.localPosition - new Vector3(0f, 150f, 0f),
                    new Vector2(0.06f, 0.1f),
                    2f,
                    false,
                    false);
                LevelManager.Instance.OnChangeTower += OnTowerChanged;
                return;
            }
            
            // Đã quá wave 1 rồi thì complete luôn
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