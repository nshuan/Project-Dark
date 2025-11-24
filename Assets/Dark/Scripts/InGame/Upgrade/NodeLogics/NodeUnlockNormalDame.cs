using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeUnlockNormalDame : INodeActivateLogic
    {
        public NodeBonusSkill bonus;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.skillBonus.unlockedNormalDame = true;
            bonus.ActivateNode(level, ref bonusInfo);
        }

        public string GetDisplayValue(int level)
        {
            return bonus.GetDisplayValue(level);
        }

        public int MaxLevel => bonus.MaxLevel;
    }
}