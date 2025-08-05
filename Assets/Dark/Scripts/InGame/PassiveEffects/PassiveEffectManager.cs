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
                if (Random.Range(0f, 1f) <= effectConfig.chance)
                {
                    pool.Get(effectConfig.passivePrefab, effectConfig.effectId, null, true)
                        .TriggerEffect(effectConfig.effectId, target, 
                            LevelUtility.GetPassiveSize(effectConfig.logicType, effectConfig.size), 
                            LevelUtility.GetPassiveValue(effectConfig.logicType, effectConfig.value), 
                            LevelUtility.GetPassiveStagger(effectConfig.logicType, effectConfig.stagger), 
                            pool);

                    cooldownEffectMap[triggerType][effectConfig.logicType] = true;
                    StartCoroutine(IECooldown(effectConfig.cooldown, () => cooldownEffectMap[triggerType][effectConfig.logicType] = false));
                    
                    CombatActions.OnEffectTriggered?.Invoke(triggerType, effectConfig.logicType, effectConfig.cooldown);
                }
            }
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