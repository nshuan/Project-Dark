using System;
using System.Collections.Generic;
using System.Globalization;
using InGame.CounterConfig;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounterBonus : INodeActivateLogic
    {
        public NodeTowerCounter.CounterType counterType;
        public BonusType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.Cooldown:
                    bonusInfo.towerCounterCooldownMultiplier ??= new Dictionary<NodeTowerCounter.CounterType, float>();
                    bonusInfo.towerCounterCooldownPlus ??= new Dictionary<NodeTowerCounter.CounterType, float>();
                    if (isMultiply)
                    {
                        if (!bonusInfo.towerCounterCooldownMultiplier.TryAdd(counterType, 0f))
                            bonusInfo.towerCounterCooldownMultiplier[counterType] += value[level - 1];
                    }
                    else
                    {
                        if (!bonusInfo.towerCounterCooldownPlus.TryAdd(counterType, 0f))
                            bonusInfo.towerCounterCooldownPlus[counterType] += (int)value[level - 1];
                    }
                    break;
                case BonusType.Damage:
                    bonusInfo.towerCounterDamagePlus += value[level - 1];
                    break;
            }
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.towerCounterCooldownMultiplier ??= new Dictionary<NodeTowerCounter.CounterType, float>();
            bonusInfo.towerCounterCooldownPlus ??= new Dictionary<NodeTowerCounter.CounterType, float>();
            var before = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    before = $"{LevelUtility.GetTowerCounterCooldown(counterType, TowerCounterManifest.Get(counterType).cooldown).ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Damage:
                    before = $"{LevelUtility.GetTowerCounterDamage(TowerCounterManifest.Get(counterType).damage).ToString(CultureInfo.InvariantCulture)}";
                    break;
            }
            if (level <= 0 || level > value.Length) return (before, before);
            
            bonusInfo.towerCounterCooldownPlus.TryAdd(counterType, 0f);
            bonusInfo.towerCounterCooldownMultiplier.TryAdd(counterType, 0f);
            var tempCooldownPlus = bonusInfo.towerCounterCooldownPlus[counterType];
            var tempCooldownMultiplier = bonusInfo.towerCounterCooldownMultiplier[counterType];
            var tempDamagePlus = bonusInfo.towerCounterDamagePlus;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    after = $"{LevelUtility.GetTowerCounterCooldown(counterType, TowerCounterManifest.Get(counterType).cooldown).ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Damage:
                    after = $"{LevelUtility.GetTowerCounterDamage(TowerCounterManifest.Get(counterType).damage).ToString(CultureInfo.InvariantCulture)}";
                    break;
            }
            bonusInfo.towerCounterCooldownPlus[counterType] = tempCooldownPlus;
            bonusInfo.towerCounterCooldownMultiplier[counterType] = tempCooldownMultiplier;
            bonusInfo.towerCounterDamagePlus = tempDamagePlus; 
            
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;

            return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            Cooldown,
            Damage
        }
    }
}