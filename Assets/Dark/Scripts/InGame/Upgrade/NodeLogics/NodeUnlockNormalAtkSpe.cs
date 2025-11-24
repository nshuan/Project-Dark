using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeUnlockNormalAtkSpe : INodeActivateLogic
    {
        public NodeProjectileActivateAction action;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.skillBonus.unlockedNormalAtkSpe = true;
            action.ActivateNode(level, ref bonusInfo);
        }

        public string GetDisplayValue(int level)
        {
            return action.GetDisplayValue(level);
        }

        public int MaxLevel => action.MaxLevel;
    }
}