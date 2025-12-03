using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial.Steps
{
    public class UITutorialStepMoveCooldown : UIAbstractTutorialStepInGame
    {
        [SerializeField] private GameObject objInstruction;

        [Space] [Header("Config")]
        [SerializeField] private int levelToShow; 
        
        public override bool IsValid()
        {
            if (LevelManager.Instance.Level.level == levelToShow) return true;
            return false;
        }

        public override void Setup()
        {
            objInstruction.SetActive(true);
            CombatActions.OnMoveCooldownComplete += OnMoveCooldownComplete;
        }

        private void OnMoveCooldownComplete()
        {
            CombatActions.OnMoveCooldownComplete -= OnMoveCooldownComplete;
            OnComplete?.Invoke();
        }
    }
}