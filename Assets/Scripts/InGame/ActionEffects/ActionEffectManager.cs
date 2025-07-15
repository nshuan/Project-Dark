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
    public class ActionEffectManager : SerializedMonoSingleton<ActionEffectManager>
    {
        [NonSerialized, OdinSerialize] private Dictionary<EffectTriggerType, Dictionary<EffectType, ActionEffectConfig>> effectConfigsMap;
        [SerializeField] private ActionEffectPool pool;
        
        private Dictionary<EffectTriggerType, List<EffectType>> possibleEffectMap;
        private Dictionary<EffectTriggerType, Dictionary<EffectType, bool>> cooldownEffectMap;

        protected override void Awake()
        {
            base.Awake();
            possibleEffectMap = new Dictionary<EffectTriggerType, List<EffectType>>()
            {
                { EffectTriggerType.DameByNormalAttack , new List<EffectType>() },
                { EffectTriggerType.DameByChargeAttack , new List<EffectType>() },
                { EffectTriggerType.DameByMoveSKill , new List<EffectType>() },
                { EffectTriggerType.TowerTakeDame , new List<EffectType>() }
            };
            cooldownEffectMap = new Dictionary<EffectTriggerType, Dictionary<EffectType, bool>>();
            foreach (EffectTriggerType triggerType in Enum.GetValues(typeof(EffectTriggerType)))
            {
                var effectMap = new Dictionary<EffectType, bool>();
                foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
                {
                    effectMap[effectType] = false;
                }
                cooldownEffectMap.Add(triggerType, effectMap);
            }
            
            UpgradeManager.Instance.OnActivated += OnBonusActivated;
        }

        private void OnBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            foreach (var pair in bonusInfo.effectsMapByTriggerType)
            {
                foreach (var effect in pair.Value)
                {
                    possibleEffectMap[pair.Key].Add(effect);
                }
            }
        }

        #region Trigger

        public void TriggerEffect(EffectTriggerType triggerType, IEffectTarget target)
        {
            if (possibleEffectMap[triggerType] == null) return;
            foreach (var effectConfig in possibleEffectMap[triggerType].Select(effectType => effectConfigsMap[triggerType][effectType]))
            {
                // Skip if in cooldown
                if (cooldownEffectMap[triggerType][effectConfig.logicType]) continue;
                
                // Calculate chance
                if (Random.Range(0f, 1f) <= effectConfig.chance)
                {
                    pool.Get(effectConfig.effectPrefab, effectConfig.effectId, null, true)
                        .TriggerEffect(effectConfig.effectId, target, effectConfig.size, effectConfig.value, effectConfig.stagger, pool);

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

    public enum EffectTriggerType
    {
        DameByNormalAttack,
        DameByChargeAttack,
        DameByMoveSKill,
        TowerTakeDame
    }

    public enum EffectType
    {
        Explosion,
        Lightning,
        Burning,
        Thunder
    }
}