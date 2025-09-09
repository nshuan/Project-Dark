using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusPassive : INodeActivateLogic
    {
        public BonusType bonusType;
        public PassiveType passiveType;
        public float[] value;
        public bool isMultiply;
        public string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;
            
            
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }

        public enum BonusType
        {
            Damage,
            Size
        }
    }
}