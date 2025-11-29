using System;
using System.Collections.Generic;
using InGame.ChargeConfig;
using UnityEngine.Serialization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeUnlockSkill : INodeActivateLogic
    {
        public BonusUnlockSkillType unlockType;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            switch (unlockType)
            {
                case BonusUnlockSkillType.NormalDamage:
                    bonusInfo.skillBonus.unlockedNormalDame = true;
                    break;  
                case BonusUnlockSkillType.NormalAtkSpe:
                    bonusInfo.skillBonus.unlockedNormalAtkSpe = true;
                    break;
                case BonusUnlockSkillType.ChargeBullet:
                    bonusInfo.skillBonus.unlockedChargeBullet = true;
                    break;
                case BonusUnlockSkillType.ChargeSize:
                    bonusInfo.skillBonus.unlockedChargeSize = true;
                    bonusInfo.skillBonus.projectileChargeHitActions ??= new List<IProjectileHit>();
                    bonusInfo.skillBonus.projectileChargeHitActions.Add(new ProjectileHitBlossom()
                    {
                        projectile = ProjectileManifest.Get(0),
                        bulletAmount = (int)PlayerChargeManifest.Get(ChargeType.Size).value,
                        blossomSize = PlayerChargeManifest.Get(ChargeType.Size).range
                    });
                    break;
                case BonusUnlockSkillType.MoveFlash:
                    bonusInfo.unlockedMoveToTower ??= new List<int>();
                    bonusInfo.unlockedMoveToTower.Add(1);
                    break;
                case BonusUnlockSkillType.MoveDash:
                    bonusInfo.unlockedMoveToTower ??= new List<int>();
                    bonusInfo.unlockedMoveToTower.Add(2);
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            return "";
        }

        public (string, string) GetBeforeAfterValue(int level)
        {
            return ("", "");
        }

        public int MaxLevel => 1;

        public enum BonusUnlockSkillType
        {
            NormalDamage,
            NormalAtkSpe,
            ChargeBullet,
            ChargeSize,
            MoveFlash,
            MoveDash
        }
    }
}