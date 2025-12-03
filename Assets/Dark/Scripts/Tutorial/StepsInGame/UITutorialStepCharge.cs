using System;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial.Steps
{
    public class UITutorialStepCharge : UIAbstractTutorialStepInGame
    {
        [SerializeField] private GameObject objInstruction;
        
        public override bool IsValid()
        {
            return LevelUtility.BonusInfo.skillBonus.unlockedChargeBullet || LevelUtility.BonusInfo.skillBonus.unlockedChargeSize;
        }

        public override void Setup()
        {
            objInstruction.SetActive(true);
            CombatActions.OnAttackCharge += OnAttackCharge;
        }

        private void OnAttackCharge(float cooldown)
        {
            OnComplete?.Invoke();
        }

        private void Update()
        {
            if (!objInstruction.activeInHierarchy) return;
            objInstruction.transform.position = Input.mousePosition;
        }
    }
}