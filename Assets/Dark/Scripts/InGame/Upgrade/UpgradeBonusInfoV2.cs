using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class UpgradeBonusInfoV2
    {
        public UpgradeBonusUnlockSkillV2 bonusUnlockSkill;
        public UpgradeBonusBaseV2 bonusBase;
        public UpgradeBonusNormalAttackV2 bonusNormalAttack;
        public UpgradeBonusChargeAttackV2 bonusChargeAttack;
        public UpgradeBonusMoveV2 bonusMove;
        public UpgradeBonusCounterV2 bonusCounter;
        public UpgradeBonusPassiveV2 bonusPassiveNormalAttack;
        public UpgradeBonusPassiveV2 bonusPassiveChargeAttack;
        public UpgradeBonusPassiveV2 bonusPassiveMove;
        public UpgradeBonusPassiveV2 bonusPassiveCounter;

        public UpgradeBonusInfoV2()
        {
            bonusUnlockSkill = new UpgradeBonusUnlockSkillV2();
            bonusBase = new UpgradeBonusBaseV2();
            bonusNormalAttack = new UpgradeBonusNormalAttackV2();
            bonusChargeAttack = new UpgradeBonusChargeAttackV2();
            bonusMove = new UpgradeBonusMoveV2();
            bonusCounter = new UpgradeBonusCounterV2();
            bonusPassiveNormalAttack = new UpgradeBonusPassiveV2();
            bonusPassiveChargeAttack = new UpgradeBonusPassiveV2();
            bonusPassiveMove = new UpgradeBonusPassiveV2();
            bonusPassiveCounter = new UpgradeBonusPassiveV2();
        }
    }

    [Serializable]
    public class UpgradeBonusUnlockSkillV2
    {
        public bool unlockNormalAttackPiercing;
        public bool unlockNormalAttackBullet;
        public bool unlockChargeAttackBullet;
        public bool unlockChargeAttackSize;
        public bool unlockMoveFlash;
        public bool unlockMoveDash;
        public bool unlockCounterPiercing;
        public bool unlockCounterSlash;
        public bool unlockPassiveNormalLightning;
        public bool unlockPassiveNormalExplosive;
        public bool unlockPassiveNormalBurning;
        public bool unlockPassiveNormalThunder;
        public bool unlockPassiveChargeLightning;
        public bool unlockPassiveChargeExplosive;
        public bool unlockPassiveChargeBurning;
        public bool unlockPassiveChargeThunder;
        public bool unlockPassiveMoveLightning;
        public bool unlockPassiveMoveExplosive;
        public bool unlockPassiveMoveBurning;
        public bool unlockPassiveMoveThunder;
        public bool unlockPassiveCounterLightning;
        public bool unlockPassiveCounterExplosive;
        public bool unlockPassiveCounterBurning;
        public bool unlockPassiveCounterThunder;
    }
    
    [Serializable]
    public class UpgradeBonusBaseV2
    {
        public UpgradeBonusStatV2 bonusHp;
        public UpgradeBonusStatV2 bonusShield;
        public UpgradeBonusStatV2 bonusDmg;
        public UpgradeBonusStatV2 bonusCooldown;
        public UpgradeBonusStatV2 bonusRegen;
        public UpgradeBonusStatV2 bonusLifeLeech;
        public UpgradeBonusStatV2 bonusCritDmg;
        public UpgradeBonusStatV2 bonusCritRate;
        public UpgradeBonusStatV2 bonusStagger;
        public UpgradeBonusStatV2 bonusDmgBoss;
        public UpgradeBonusStatV2 bonusVestigeDrop;
        public UpgradeBonusStatV2 bonusVestigeDoubleChance;
        public UpgradeBonusStatV2 bonusVestigeTripleChance;
        public UpgradeBonusStatV2 bonusVestigeCollectSize;
        public UpgradeBonusStatV2 bonusExpDrop;
    }

    [Serializable]
    public class UpgradeBonusNormalAttackV2
    {
        public UpgradeBonusStatV2 bonusNormalAttackDmg;
        public UpgradeBonusStatV2 bonusNormalAttackCooldown;
        public UpgradeBonusStatV2 bonusNormalAttackRange;
        public UpgradeBonusStatV2 bonusNormalAttackSize;
        public UpgradeBonusStatV2 bonusPiercingDmg;
        public UpgradeBonusStatV2 bonusPiercingAmount;
        public UpgradeBonusStatV2 bonusBulletDmg;
        public UpgradeBonusStatV2 bonusBulletAmount;
    }

    [Serializable]
    public class UpgradeBonusChargeAttackV2
    {
        public UpgradeBonusStatV2 bonusChargeCooldown;
        public UpgradeBonusStatV2 bonusChargeTime;
        public UpgradeBonusStatV2 bonusChargeDmg;
        public UpgradeBonusStatV2 bonusBulletAmount;
        public UpgradeBonusStatV2 bonusSizeAmount;
    }

    [Serializable]
    public class UpgradeBonusMoveV2
    {
        public UpgradeBonusStatV2 bonusMoveDmg;
        public UpgradeBonusStatV2 bonusMoveCooldown;
        public UpgradeBonusStatV2 bonusFlashSize;
        public UpgradeBonusStatV2 bonusDashSize;
    }

    [Serializable]
    public class UpgradeBonusCounterV2
    {
        public UpgradeBonusStatV2 bonusCounterDmg;
        public UpgradeBonusStatV2 bonusCounterCooldown;
        public UpgradeBonusStatV2 bonusPiercingAmount;
        public UpgradeBonusStatV2 bonusSlashSize;
    }

    [Serializable]
    public class UpgradeBonusPassiveV2
    {
        public UpgradeBonusStatV2 bonusLightningDmg;
        public UpgradeBonusStatV2 bonusLightningRate;
        public UpgradeBonusStatV2 bonusLightningAmount;
        public UpgradeBonusStatV2 bonusExplosiveDmg;
        public UpgradeBonusStatV2 bonusExplosiveRate;
        public UpgradeBonusStatV2 bonusExplosiveSize;
        public UpgradeBonusStatV2 bonusBurningDmg;
        public UpgradeBonusStatV2 bonusBurningRate;
        public UpgradeBonusStatV2 bonusBurningDuration;
        public UpgradeBonusStatV2 bonusThunderDmg;
        public UpgradeBonusStatV2 bonusThunderRate;
        public UpgradeBonusStatV2 bonusThunderExecutionChance;
    }

    [Serializable]
    public struct UpgradeBonusStatV2
    {
        public float add;
        public float mul;
        public int addInt => Mathf.RoundToInt(add);
    }
}