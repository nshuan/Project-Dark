using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusHpRegen : INodeActivateLogic
    {
        public BonusType bonusType;
        public float[] value;
        public bool isMultiply;
        public string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    bonusInfo.toleranceRegenPerSecond += (int)value[level - 1];
                    break;
                case BonusType.OnEnemyDied:
                    bonusInfo.toleranceRegenWhenKill += (int)value[level - 1];
                    break;
            }
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }

        public enum BonusType
        {
            AutoRegenerate,
            OnEnemyDied
        }
    }
}