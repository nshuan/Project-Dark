using System;
using System.Collections.Generic;
using InGame;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatUnlock : MonoBehaviour
    {
        public Button[] btnUnlockAttackPassive;
        public Button[] btnUnlockChargePassive;
        public Button[] btnUnlockMovePassive;
        public Button[] btnUnlockCounterPassive;

        public Button btnUnlockChargeSize;
        public Button btnUnlockChargeBullet;

        private void OnEnable()
        {
            // Normal attack passive
            for (var i = 0; i < btnUnlockAttackPassive.Length; i++)
            {
                var passiveType = (PassiveType)i;
                var targetGraphic = btnUnlockAttackPassive[i].targetGraphic;
#if HOT_CHEAT
                targetGraphic.color = ExistPassive(PassiveTriggerType.DameByNormalAttack, passiveType)
                    ? Color.green : Color.red;
#endif
                btnUnlockAttackPassive[i].onClick.RemoveAllListeners();
                btnUnlockAttackPassive[i].onClick.AddListener(() =>
                {
#if HOT_CHEAT
                    AddOrRemovePassive(PassiveTriggerType.DameByNormalAttack, passiveType);
                    targetGraphic.color = ExistPassive(PassiveTriggerType.DameByNormalAttack, passiveType)
                        ? Color.green : Color.red;
#endif
                });
            }
            
            // Charge attack passive
            for (var i = 0; i < btnUnlockChargePassive.Length; i++)
            {
                var passiveType = (PassiveType)i;
                var targetGraphic = btnUnlockChargePassive[i].targetGraphic;
#if HOT_CHEAT
                targetGraphic.color = ExistPassive(PassiveTriggerType.DameByChargeAttack, passiveType)
                    ? Color.green : Color.red;
#endif
                btnUnlockChargePassive[i].onClick.RemoveAllListeners();
                btnUnlockChargePassive[i].onClick.AddListener(() =>
                {
#if HOT_CHEAT
                    AddOrRemovePassive(PassiveTriggerType.DameByChargeAttack, passiveType);
                    targetGraphic.color = ExistPassive(PassiveTriggerType.DameByChargeAttack, passiveType)
                        ? Color.green : Color.red;
#endif
                });
            }
            
            // Move passive
            for (var i = 0; i < btnUnlockMovePassive.Length; i++)
            {
                var passiveType = (PassiveType)i;
                var targetGraphic = btnUnlockMovePassive[i].targetGraphic;
#if HOT_CHEAT
                targetGraphic.color = ExistPassive(PassiveTriggerType.DameByMoveSKill, passiveType)
                    ? Color.green : Color.red;
#endif
                btnUnlockMovePassive[i].onClick.RemoveAllListeners();
                btnUnlockMovePassive[i].onClick.AddListener(() =>
                {
#if HOT_CHEAT
                    AddOrRemovePassive(PassiveTriggerType.DameByMoveSKill, passiveType);
                    targetGraphic.color = ExistPassive(PassiveTriggerType.DameByMoveSKill, passiveType)
                        ? Color.green : Color.red;
#endif
                });
            }
            
            // Tower Counter
            for (var i = 0; i < btnUnlockCounterPassive.Length; i++)
            {
                var passiveType = (PassiveType)i;
                var targetGraphic = btnUnlockCounterPassive[i].targetGraphic;
#if HOT_CHEAT
                targetGraphic.color = ExistPassive(PassiveTriggerType.TowerTakeDame, passiveType)
                    ? Color.green : Color.red;
#endif
                btnUnlockCounterPassive[i].onClick.RemoveAllListeners();
                btnUnlockCounterPassive[i].onClick.AddListener(() =>
                {
#if HOT_CHEAT
                    AddOrRemovePassive(PassiveTriggerType.TowerTakeDame, passiveType);
                    targetGraphic.color = ExistPassive(PassiveTriggerType.TowerTakeDame, passiveType)
                        ? Color.green : Color.red;
#endif
                });
            }
            
            btnUnlockChargeSize.targetGraphic.color = LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackSize? Color.green : Color.red;
            btnUnlockChargeSize.onClick.RemoveAllListeners();
            btnUnlockChargeSize.onClick.AddListener(() =>
            {
                var bonusInfo = LevelUtilityV2.BonusInfo;
                bonusInfo.bonusUnlockSkill.unlockChargeAttackSize = !bonusInfo.bonusUnlockSkill.unlockChargeAttackSize;
                btnUnlockChargeSize.targetGraphic.color = bonusInfo.bonusUnlockSkill.unlockChargeAttackSize ? Color.green : Color.red;
            });
            
            btnUnlockChargeBullet.targetGraphic.color = LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackBullet ? Color.green : Color.red;
            btnUnlockChargeBullet.onClick.RemoveAllListeners();
            btnUnlockChargeBullet.onClick.AddListener(() =>
            {
                var bonusInfo = LevelUtilityV2.BonusInfo;
                bonusInfo.bonusUnlockSkill.unlockChargeAttackBullet = !bonusInfo.bonusUnlockSkill.unlockChargeAttackBullet;
                btnUnlockChargeBullet.targetGraphic.color = bonusInfo.bonusUnlockSkill.unlockChargeAttackBullet ? Color.green : Color.red;
            });
        }

        private bool ExistPassive(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            return LevelUtilityV2.IsUnlockedPassive(triggerType, passiveType);
        }

        private void AddOrRemovePassive(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            // LevelUtilityV2.BonusInfo ??= new UpgradeBonusInfoV2();
            // LevelUtilityV2.BonusInfo.passiveMapByTriggerType ??= new Dictionary<PassiveTriggerType, List<PassiveType>>();
            // if (!LevelUtilityV2.BonusInfo.passiveMapByTriggerType.ContainsKey(triggerType)) 
            //     LevelUtilityV2.BonusInfo.passiveMapByTriggerType.Add(triggerType, new List<PassiveType>());
            // if (!LevelUtilityV2.BonusInfo.passiveMapByTriggerType[triggerType].Contains(passiveType))
            //     LevelUtilityV2.BonusInfo.passiveMapByTriggerType[triggerType].Add(passiveType);
            // else LevelUtilityV2.BonusInfo.passiveMapByTriggerType[triggerType].Remove(passiveType);

#if HOT_CHEAT
            UpgradeManager.Instance.CheatUpdateBonusInfo(LevelUtilityV2.BonusInfo);
#endif
        }
    }
}