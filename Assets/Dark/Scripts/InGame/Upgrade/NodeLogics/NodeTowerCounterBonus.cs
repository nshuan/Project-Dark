using System;
using System.Collections.Generic;

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
                    bonusInfo.towerCounterCooldownPlus ??= new Dictionary<NodeTowerCounter.CounterType, float>();
                    if (!bonusInfo.towerCounterCooldownPlus.TryAdd(counterType, 0f))
                        bonusInfo.towerCounterCooldownPlus[counterType] += (int)value[level - 1];
                    break;
                case BonusType.Damage:
                    bonusInfo.towerCounterDamagePlus += value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            return (value[level] * 100).ToString();
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            Cooldown,
            Damage
        }
    }
}