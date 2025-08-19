using System.Collections.Generic;
using InGame.Upgrade;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeLogicGenerator
    {
        public static INodeActivateLogic[] Generate(List<NodeLogicInfo> infos)
        {
            return null;
        }
    }

    public struct NodeLogicInfo
    {
        public string key;
        public List<string> value;
        public string isMul;
    }
    
    public enum LogicType
    {
        UnlockDash,
        UnlockFlash,
        UnlockCounter,
        UnlockAttackPassive,
        UnlockChargePassive,
        UnlockMovePassive,
        UnlockCounterPassive,
        UnlockAttackDame,
        UnlockAttackSpeed,
        UnlockChargeSize,
        UnlockChargeBullet,
        BonusDropRate,
        BonusTotalDamage,
        BonusCriticalRate,
        BonusCriticalDamage,
        BonusAttackSize,
        BonusTotalCooldown,
        BonusBps, // Bullet per shot
        BonusAttackSpeed,
        BonusHp,
        BonusHpRegenPerSec,
        BonusHpRegenOnKill,
        BonusAttackRange,
        BonusMoveCooldown,
        BonusMoveCastTime,
        BonusStagger,
        TempBonusAttackSpeed,
        TempBonusDamage,
        TempBonusCriticalRate,
        TempBonusCriticalDamage,
        BonusChargeTime,
        BonusDashCooldown,
        BonusDashSize,
        BonusDashDamage,
        BonusFlashCooldown,
        BonusFlashSize,
        BonusFlashDamage,
        BonusCounterCooldown,
        BonusCounterDamage,
        BonusPassiveDamage,
        BonusPassiveSize
    }
}