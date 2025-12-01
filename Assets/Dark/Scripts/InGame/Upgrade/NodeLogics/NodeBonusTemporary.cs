using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusTemporary : INodeActivateLogic
    {
        public BonusTemporaryType bonusType;
        public float[] value;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusTemporaryType.DamageOnKill:
                    bonusInfo.tempDamageBonusOnKill ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempDamageBonusOnKill.bonusValue += value[level - 1];
                    break;
                case BonusTemporaryType.DamageOnMove:
                    bonusInfo.tempDamageBonusOnMove ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempDamageBonusOnMove.bonusValue += value[level - 1];
                    break;
                case BonusTemporaryType.AtkSpeOnKill:
                    bonusInfo.tempAtkSpeBonusOnKill ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempAtkSpeBonusOnKill.bonusValue += value[level - 1];
                    break;
                case BonusTemporaryType.AtkSpeOnMove:
                    bonusInfo.tempAtkSpeBonusOnMove ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempAtkSpeBonusOnMove.bonusValue += value[level - 1];
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
                case BonusTemporaryType.DamageOnKill:
                    before = $"+{(bonusInfo.tempDamageBonusOnKill.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.DamageOnMove:
                    before = $"+{(bonusInfo.tempDamageBonusOnMove.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.AtkSpeOnKill:
                    before = $"+{(bonusInfo.tempAtkSpeBonusOnKill.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.AtkSpeOnMove:
                    before = $"+{(bonusInfo.tempAtkSpeBonusOnMove.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
            }
            if (level > value.Length)
                return (before, before);
            var tempDamageBonusOnKill = bonusInfo.tempDamageBonusOnKill.bonusValue;
            var tempDamageBonusOnMove = bonusInfo.tempDamageBonusOnMove.bonusValue;
            var tempAtkSpeBonusOnKill = bonusInfo.tempAtkSpeBonusOnKill.bonusValue;
            var tempAtkSpeBonusOnMove = bonusInfo.tempAtkSpeBonusOnMove.bonusValue;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusTemporaryType.DamageOnKill:
                    after = $"+{(bonusInfo.tempDamageBonusOnKill.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.DamageOnMove:
                    after = $"+{(bonusInfo.tempDamageBonusOnMove.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.AtkSpeOnKill:
                    after = $"+{(bonusInfo.tempAtkSpeBonusOnKill.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusTemporaryType.AtkSpeOnMove:
                    after = $"+{(bonusInfo.tempAtkSpeBonusOnMove.bonusValue * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
            }
            bonusInfo.tempDamageBonusOnKill.bonusValue = tempDamageBonusOnKill;
            bonusInfo.tempDamageBonusOnMove.bonusValue = tempDamageBonusOnMove;
            bonusInfo.tempAtkSpeBonusOnKill.bonusValue = tempAtkSpeBonusOnKill;
            bonusInfo.tempAtkSpeBonusOnMove.bonusValue = tempAtkSpeBonusOnMove;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
        }
        
        public int MaxLevel => value.Length;

        public enum BonusTemporaryType
        {
            DamageOnKill,
            DamageOnMove,
            AtkSpeOnKill,
            AtkSpeOnMove
        }
    }
}