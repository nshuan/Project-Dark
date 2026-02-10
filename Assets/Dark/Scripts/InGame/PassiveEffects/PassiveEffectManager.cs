using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using InGame.Upgrade;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class PassiveEffectManager : SerializedMonoSingleton<PassiveEffectManager>
    {
        [NonSerialized, OdinSerialize] private Dictionary<PassiveTriggerType, Dictionary<PassiveType, PassiveConfig>> effectConfigsMap;
        [SerializeField] private PassiveEffectPool pool;
        
        private Dictionary<PassiveTriggerType, List<PassiveType>> possibleEffectMap;
        private Dictionary<PassiveTriggerType, Dictionary<PassiveType, bool>> cooldownEffectMap;

        protected override void Awake()
        {
            base.Awake();
            
            // Setup passive config map in LevelUtility
            LevelUtilityV2.PassiveConfigsMap = new Dictionary<PassiveTriggerType, Dictionary<PassiveType, PassiveConfig>>();
            foreach (var pair in effectConfigsMap)
            {
                var subDict = new Dictionary<PassiveType, PassiveConfig>();
                foreach (var subPair in pair.Value)
                    subDict.Add(subPair.Key, subPair.Value);
                LevelUtilityV2.PassiveConfigsMap.Add(pair.Key, subDict);
            }
            
            cooldownEffectMap = new Dictionary<PassiveTriggerType, Dictionary<PassiveType, bool>>();
            foreach (PassiveTriggerType triggerType in Enum.GetValues(typeof(PassiveTriggerType)))
            {
                var effectMap = new Dictionary<PassiveType, bool>();
                foreach (PassiveType effectType in Enum.GetValues(typeof(PassiveType)))
                {
                    effectMap[effectType] = false;
                }
                cooldownEffectMap.Add(triggerType, effectMap);
            }
            
            UpgradeManager.Instance.OnActivated += OnBonusActivated;
        }

        protected override void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnBonusActivated;
            base.OnDestroy();
        }

        private void OnBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            possibleEffectMap = new Dictionary<PassiveTriggerType, List<PassiveType>>()
            {
                { PassiveTriggerType.DameByNormalAttack , new List<PassiveType>() },
                { PassiveTriggerType.DameByChargeAttack , new List<PassiveType>() },
                { PassiveTriggerType.DameByMoveSKill , new List<PassiveType>() },
                { PassiveTriggerType.TowerTakeDame , new List<PassiveType>() }
            };
            
            if (bonusInfo.bonusUnlockSkill.unlockPassiveNormalLightning) possibleEffectMap[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Lightning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveNormalExplosive) possibleEffectMap[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Explosion);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveNormalBurning) possibleEffectMap[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Burning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveNormalThunder) possibleEffectMap[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Thunder);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveChargeLightning) possibleEffectMap[PassiveTriggerType.DameByChargeAttack].Add(PassiveType.Lightning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveChargeExplosive) possibleEffectMap[PassiveTriggerType.DameByChargeAttack].Add(PassiveType.Explosion);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveChargeBurning) possibleEffectMap[PassiveTriggerType.DameByChargeAttack].Add(PassiveType.Burning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveChargeThunder) possibleEffectMap[PassiveTriggerType.DameByChargeAttack].Add(PassiveType.Thunder);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveMoveLightning) possibleEffectMap[PassiveTriggerType.DameByMoveSKill].Add(PassiveType.Lightning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveMoveExplosive) possibleEffectMap[PassiveTriggerType.DameByMoveSKill].Add(PassiveType.Explosion);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveMoveBurning) possibleEffectMap[PassiveTriggerType.DameByMoveSKill].Add(PassiveType.Burning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveMoveThunder) possibleEffectMap[PassiveTriggerType.DameByMoveSKill].Add(PassiveType.Thunder);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveCounterLightning) possibleEffectMap[PassiveTriggerType.TowerTakeDame].Add(PassiveType.Lightning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveCounterExplosive) possibleEffectMap[PassiveTriggerType.TowerTakeDame].Add(PassiveType.Explosion);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveCounterBurning) possibleEffectMap[PassiveTriggerType.TowerTakeDame].Add(PassiveType.Burning);
            if (bonusInfo.bonusUnlockSkill.unlockPassiveCounterThunder) possibleEffectMap[PassiveTriggerType.TowerTakeDame].Add(PassiveType.Thunder);
        }

        #region Trigger

        public void TriggerEffect(PassiveTriggerType triggerType, IEffectTarget target)
        {
            if (possibleEffectMap[triggerType] == null) return;
            foreach (var effectConfig in possibleEffectMap[triggerType].Select(effectType => effectConfigsMap[triggerType][effectType]))
            {
                // Skip if in cooldown
                if (cooldownEffectMap[triggerType][effectConfig.logicType]) continue;
                
                // Skip if explosion on alive enemy
                if (effectConfig.logicType == PassiveType.Explosion && !target.IsEffectTargetDead) continue;
                
                // Calculate chance
                if (RandomUtil.Range(0f, 1f) <= LevelUtilityV2.GetPassiveChance(triggerType, effectConfig.logicType))
                {
                    pool.Get(effectConfig.passivePrefab, effectConfig.passiveId, null, false)
                        .TriggerEffect(effectConfig.passiveId, target, 
                            LevelUtilityV2.GetPassiveSize(triggerType, effectConfig.logicType), 
                            LevelUtilityV2.GetPassiveValue(triggerType, effectConfig.logicType), 
                            LevelUtilityV2.GetPassiveStagger(triggerType, effectConfig.logicType), 
                            pool,
                            effectConfig.GetAdditionalParams());

                    cooldownEffectMap[triggerType][effectConfig.logicType] = true;
                    var cooldown = LevelUtilityV2.GetPassiveCooldown(triggerType, effectConfig.logicType);
                    StartCoroutine(IECooldown(cooldown, () => cooldownEffectMap[triggerType][effectConfig.logicType] = false));
                    
                    CombatActions.OnEffectTriggered?.Invoke(triggerType, effectConfig.logicType, cooldown);
                }
            }
        }
        
        public void ForceTriggerEffect(PassiveTriggerType triggerType, PassiveType passiveType, IEffectTarget target)
        {
            var passiveConfig = effectConfigsMap[triggerType][passiveType];
            pool.Get(passiveConfig.passivePrefab, passiveConfig.passiveId, null, false)
                .TriggerEffect(passiveConfig.passiveId, target, 
                    LevelUtilityV2.GetPassiveSize(triggerType, passiveType), 
                    LevelUtilityV2.GetPassiveValue(triggerType, passiveType), 
                    LevelUtilityV2.GetPassiveStagger(triggerType, passiveType), 
                    pool);
        }

        private IEnumerator IECooldown(float cooldown, Action completeCallback)
        {
            yield return new WaitForSeconds(cooldown);
            completeCallback?.Invoke();
        }

        #endregion
    }

    public enum PassiveTriggerType
    {
        DameByNormalAttack,
        DameByChargeAttack,
        DameByMoveSKill,
        TowerTakeDame
    }

    public enum PassiveType
    {
        Explosion,
        Lightning,
        Burning,
        Thunder
    }
}