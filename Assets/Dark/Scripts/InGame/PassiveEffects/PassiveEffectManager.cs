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
            LevelUtility.EffectConfigsMap = new Dictionary<PassiveTriggerType, Dictionary<PassiveType, PassiveConfig>>();
            foreach (var pair in effectConfigsMap)
            {
                var subDict = new Dictionary<PassiveType, PassiveConfig>();
                foreach (var subPair in pair.Value)
                    subDict.Add(subPair.Key, subPair.Value);
                LevelUtility.EffectConfigsMap.Add(pair.Key, subDict);
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

        private void OnBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            possibleEffectMap = new Dictionary<PassiveTriggerType, List<PassiveType>>()
            {
                { PassiveTriggerType.DameByNormalAttack , new List<PassiveType>() },
                { PassiveTriggerType.DameByChargeAttack , new List<PassiveType>() },
                { PassiveTriggerType.DameByMoveSKill , new List<PassiveType>() },
                { PassiveTriggerType.TowerTakeDame , new List<PassiveType>() }
            };
            
            foreach (var pair in bonusInfo.passiveMapByTriggerType)
            {
                foreach (var effect in pair.Value)
                {
                    possibleEffectMap[pair.Key].Add(effect);
                }
            }
        }

        #region Trigger

        public void TriggerEffect(PassiveTriggerType triggerType, IEffectTarget target)
        {
            if (possibleEffectMap[triggerType] == null) return;
            foreach (var effectConfig in possibleEffectMap[triggerType].Select(effectType => effectConfigsMap[triggerType][effectType]))
            {
                // Skip if in cooldown
                if (cooldownEffectMap[triggerType][effectConfig.logicType]) continue;
                
                // Calculate chance
                if (RandomUtil.Range(0f, 1f) <= LevelUtility.GetPassiveChance(triggerType, effectConfig.logicType))
                {
                    pool.Get(effectConfig.passivePrefab, effectConfig.passiveId, null, false)
                        .TriggerEffect(effectConfig.passiveId, target, 
                            LevelUtility.GetPassiveSize(triggerType, effectConfig.logicType), 
                            LevelUtility.GetPassiveValue(triggerType, effectConfig.logicType), 
                            LevelUtility.GetPassiveStagger(triggerType, effectConfig.logicType), 
                            pool);

                    cooldownEffectMap[triggerType][effectConfig.logicType] = true;
                    var cooldown = LevelUtility.GetPassiveCooldown(triggerType, effectConfig.logicType);
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
                    LevelUtility.GetPassiveSize(triggerType, passiveType), 
                    LevelUtility.GetPassiveValue(triggerType, passiveType), 
                    LevelUtility.GetPassiveStagger(triggerType, passiveType), 
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