using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeUnlockCharge : INodeActivateLogic
    {
        public BonusUnlockChargeType unlockType;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            switch (unlockType)
            {
                case BonusUnlockChargeType.Damage:
                    bonusInfo.skillBonus.unlockedChargeDame = true;
                    break;  
                case BonusUnlockChargeType.Bullet:
                    bonusInfo.skillBonus.unlockedChargeBullet = true;
                    break;
                case BonusUnlockChargeType.Size:
                    bonusInfo.skillBonus.unlockedChargeSize = true;
                    break;
                case BonusUnlockChargeType.Range:
                    bonusInfo.skillBonus.unlockedChargeRange = true;
                    break;
            }
        }
        
        public enum BonusUnlockChargeType
        {
            Damage,
            Bullet,
            Size,
            Range
        }
    }
}