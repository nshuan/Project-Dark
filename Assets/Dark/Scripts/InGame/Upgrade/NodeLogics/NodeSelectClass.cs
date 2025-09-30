using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeSelectClass : INodeActivateLogic
    {
        public string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            
        }

        public string GetDisplayValue(int level)
        {
            return bonusDescription;
        }
    }
}