using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusPassive : INodeActivateLogic
    {
        public BonusType bonusType;
        public PassiveType passiveType;
        public float[] value;
        
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
                case BonusType.Chance:
                    bonusInfo.passiveBonusChanceMapByType ??= new Dictionary<PassiveType, float>();
                    bonusInfo.passiveBonusChanceMapByType.TryAdd(passiveType, 0);
                    bonusInfo.passiveBonusChanceMapByType.TryAdd(passiveType, 0);
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;

            if (passiveType == PassiveType.Lightning && bonusType == BonusType.Size)
                return value[level].ToString(CultureInfo.InvariantCulture);
            
            if (passiveType == PassiveType.Burning && bonusType == BonusType.Size) 
                return value[level].ToString(CultureInfo.InvariantCulture);
            
            return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
        }

        public (string, string) GetBeforeAfterValue(int level)
        {
            var before = "";
            var after = "";
            if (level < 0) return ("", "");
            if (level >= value.Length) level = value.Length - 1;
            if (level == 0)
            {
                before = "0";
                after = GetDisplayValue(level);
            }
            else
            {
                var sum = 0f;
                for (var i = 0; i < level; i++)
                    sum += value[i];

                before = (sum * 100).ToString(CultureInfo.InvariantCulture);
                after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
                
                if (passiveType == PassiveType.Lightning && bonusType == BonusType.Size)
                {
                    before = sum.ToString(CultureInfo.InvariantCulture);
                    after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
                }

                if (passiveType == PassiveType.Burning && bonusType == BonusType.Size)
                {
                    before = sum.ToString(CultureInfo.InvariantCulture);
                    after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
                }
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            Damage,
            Size,
            Chance
        }
    }
}