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

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.tempDamageBonusOnKill ??= new UpgradeBonusTempInfo();
            bonusInfo.tempDamageBonusOnMove ??= new UpgradeBonusTempInfo();
            bonusInfo.tempAtkSpeBonusOnKill ??= new UpgradeBonusTempInfo();
            bonusInfo.tempAtkSpeBonusOnMove ??= new UpgradeBonusTempInfo();
            var before = "";
            switch (bonusType)
            {
                case NodeBonusTemporary.BonusTemporaryType.DamageOnKill:
                    before = $"{bonusInfo.tempDamageBonusOnMove.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.DamageOnMove:
                    before = $"{bonusInfo.tempDamageBonusOnKill.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnKill:
                    before = $"{bonusInfo.tempAtkSpeBonusOnMove.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnMove:
                    before = $"{bonusInfo.tempAtkSpeBonusOnKill.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
            }
            if (level >= value.Length)
                return (before, before);
            var tempDamageBonusOnKill = bonusInfo.tempDamageBonusOnKill.bonusDuration;
            var tempDamageBonusOnMove = bonusInfo.tempDamageBonusOnMove.bonusDuration;
            var tempAtkSpeBonusOnKill = bonusInfo.tempAtkSpeBonusOnKill.bonusDuration;
            var tempAtkSpeBonusOnMove = bonusInfo.tempAtkSpeBonusOnMove.bonusDuration;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case NodeBonusTemporary.BonusTemporaryType.DamageOnKill:
                    before = $"{bonusInfo.tempDamageBonusOnMove.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.DamageOnMove:
                    before = $"{bonusInfo.tempDamageBonusOnKill.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnKill:
                    before = $"{bonusInfo.tempAtkSpeBonusOnMove.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case NodeBonusTemporary.BonusTemporaryType.AtkSpeOnMove:
                    before = $"{bonusInfo.tempAtkSpeBonusOnKill.bonusDuration.ToString(CultureInfo.InvariantCulture)}s";
                    break;
            }   
            bonusInfo.tempDamageBonusOnKill.bonusDuration = tempDamageBonusOnKill;
            bonusInfo.tempDamageBonusOnMove.bonusDuration = tempDamageBonusOnMove;
            bonusInfo.tempAtkSpeBonusOnKill.bonusDuration = tempAtkSpeBonusOnKill;
            bonusInfo.tempAtkSpeBonusOnMove.bonusDuration = tempAtkSpeBonusOnMove;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;
    }
}