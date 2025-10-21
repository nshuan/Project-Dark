using System;

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
                    bonusInfo.tempDamageBonusOnKill.bonusValue += (int)value[level - 1];
                    break;
                case BonusTemporaryType.DamageOnMove:
                    bonusInfo.tempDamageBonusOnMove ??= new UpgradeBonusTempInfo();
                    bonusInfo.tempDamageBonusOnMove.bonusValue += (int)value[level - 1];
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

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return (value[level] * 100).ToString();
        }

        public enum BonusTemporaryType
        {
            DamageOnKill,
            DamageOnMove,
            AtkSpeOnKill,
            AtkSpeOnMove
        }
    }
}