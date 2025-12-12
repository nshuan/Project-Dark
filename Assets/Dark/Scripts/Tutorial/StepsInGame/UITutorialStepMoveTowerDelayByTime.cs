using System.Collections;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepMoveTowerDelayByTime : UIAbstractTutorialStepInGame
    {
        [SerializeField] private GameObject objInstruction;
        
        [Space] [Header("Config")]
        [SerializeField] private int levelToShow;
        [SerializeField] private float delayShow;
        [SerializeField] private int towerToInstruct;
        
        private Coroutine coroutineDelay;
         
        public override bool IsValid()
        {
            if (LevelManager.Instance.Level.level == levelToShow) return true;
            return false;
        }

        public override void Setup()
        {
            if (coroutineDelay != null) StopCoroutine(coroutineDelay);
            coroutineDelay = StartCoroutine(IEDelayInstruction());
        }

        private IEnumerator IEDelayInstruction()
        {
            // Level đã start thì bắt đầu luôn, chưa thì đợi level start
            yield return new WaitUntil(() => LevelManager.Instance.LevelStarted);
            yield return new WaitForSeconds(delayShow);
            // Đang ở tower cần di chuyển thì complete luôn
            if (LevelManager.Instance.CurrentTower.Id == towerToInstruct)
            {
                OnComplete?.Invoke();
                yield break;
            }
            
            // Còn không thì show instruction và đợi move sang tower cần di chuyển
            objInstruction.SetActive(true);
            LevelManager.Instance.OnChangeTower += OnTowerChanged;
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