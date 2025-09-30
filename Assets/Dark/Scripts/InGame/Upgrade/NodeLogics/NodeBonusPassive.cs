using System;
using System.Collections.Generic;
using System.Linq;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusPassive : INodeActivateLogic
    {
        public BonusType bonusType;
        public PassiveType passiveType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.Damage:
                    bonusInfo.passiveBonusValueMapByType ??= new Dictionary<PassiveType, float>();
                    bonusInfo.passiveBonusValueMapByType.TryAdd(passiveType, 0);
                    bonusInfo.passiveBonusValueMapByType[passiveType] += value[level - 1];
                    break;
                case BonusType.Size:
                    bonusInfo.passiveBonusSizeMapByType ??= new Dictionary<PassiveType, float>();
                    bonusInfo.passiveBonusSizeMapByType.TryAdd(passiveType, 0);
                    bonusInfo.passiveBonusSizeMapByType[passiveType] += value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0 || level >= value.Length) return "??";
            return value[level].ToString();
        }

        public enum BonusType
        {
            Damage,
            Size
        }
    }
}