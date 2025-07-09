using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using InGame.Upgrade;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class ActionEffectManager : SerializedMonoSingleton<ActionEffectManager>
    {
        [NonSerialized, OdinSerialize] private Dictionary<EffectTriggerType, Dictionary<EffectType, ActionEffectConfig>> effectConfigsMap;
        [SerializeField] private ActionEffectPool pool;
        
        private Dictionary<EffectTriggerType, List<EffectType>> possibleEffectMap;

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
            
            possibleEffectMap[EffectTriggerType.DameByNormalAttack].Add(EffectType.Explosion);
        }

        #region Trigger

        public void TriggerEffect(EffectTriggerType triggerType, Vector2 position)
        {
            if (possibleEffectMap[triggerType] == null) return;
            foreach (var effectConfig in possibleEffectMap[triggerType].Select(effectType => effectConfigsMap[triggerType][effectType]))
            {
                pool.Get(effectConfig.effectPrefab, effectConfig.effectId, null, true)
                    .TriggerEffect(effectConfig.effectId, position, effectConfig.size, effectConfig.value, effectConfig.stagger, pool);
            }
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