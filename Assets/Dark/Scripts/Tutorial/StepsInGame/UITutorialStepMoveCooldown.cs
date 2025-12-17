using System;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
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
            actionUpdateFocus?.Invoke(
                objInstruction.transform.localPosition - new Vector3(240f, 0f, 0f),
                new Vector2(0.05f, 0.05f),
                4.4f,
                true,
                true);
            CombatActions.OnMoveCooldownComplete += OnMoveCooldownComplete;
        }

        private void OnMoveCooldownComplete()
        {
            CombatActions.OnMoveCooldownComplete -= OnMoveCooldownComplete;
            OnComplete?.Invoke();
        }

        private void Update()
        {
            if (objInstruction.activeInHierarchy)
            {
                actionUpdateFocus?.Invoke(
                    objInstruction.transform.localPosition - new Vector3(240f, 0f, 0f),
                    new Vector2(0.05f, 0.05f),
                    4.4f,
                    true,
                    true);
            }
        }
    }
}