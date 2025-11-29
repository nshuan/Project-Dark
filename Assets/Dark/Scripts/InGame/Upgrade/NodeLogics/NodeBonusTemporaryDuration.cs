using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusTemporaryDuration : INodeActivateLogic
    {
        public NodeBonusTemporary.BonusTemporaryType bonusType;
        public float[] value;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case NodeBonusTemporary.BonusTemporaryType.DamageOnKill:
                    bonusInfo.tempDamageBonusOnKill ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempDamageBonusOnKill.bonusDuration += (int)value[level - 1];
                    break;
                case NodeBonusTemporary.BonusTemporaryType.DamageOnMove:
                    bonusInfo.tempDamageBonusOnMove ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempDamageBonusOnMove.bonusDuration += (int)value[level - 1];
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnKill:
                    bonusInfo.tempAtkSpeBonusOnKill ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempAtkSpeBonusOnKill.bonusDuration += value[level - 1];
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnMove:
                    bonusInfo.tempAtkSpeBonusOnMove ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempAtkSpeBonusOnMove.bonusDuration += value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return value[level].ToString(CultureInfo.InvariantCulture);
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
                
                before = sum.ToString(CultureInfo.InvariantCulture);
                after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;
    }
}