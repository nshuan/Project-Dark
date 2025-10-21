using System;

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
            
            return value[level].ToString();
        }
    }
}