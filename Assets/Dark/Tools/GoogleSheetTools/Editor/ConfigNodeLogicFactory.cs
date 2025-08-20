using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public static class ConfigNodeLogicFactory
    {
        private static Dictionary<LogicType, Type> typeMap;

        static ConfigNodeLogicFactory()
        {
            GenerateTypeMap();
        }

        private static void GenerateTypeMap()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(ConfigNodeLogicTypeAttribute), false).Any())
                .ToList();
            
            var dupes = types.GroupBy(t => ((ConfigNodeLogicTypeAttribute)t
                .GetCustomAttributes(typeof(ConfigNodeLogicTypeAttribute), false)
                .First()).LogicType).Where(g => g.Count() > 1).ToList();

            if (dupes.Count > 0)
            {
                typeMap = null;
                throw new Exception("[Factory] There are more than 1 generator with the same logic type: \n" +
                                    string.Join(Environment.NewLine, dupes.Select(g => g.Key)));
            }
            
            typeMap = types.ToDictionary(
                t => ((ConfigNodeLogicTypeAttribute)t
                    .GetCustomAttributes(typeof(ConfigNodeLogicTypeAttribute), false)
                    .First()).LogicType,
                t => t
            );
        }
        
        public static INodeActivateLogic[] Generate(List<NodeLogicInfo> infos)
        {
            if (typeMap == null) GenerateTypeMap();
            
            var result = new List<INodeActivateLogic>();

            foreach (var info in infos)
            {
                var logicType = GetLogicType(info.key);
                if (logicType == LogicType.None)
                {
                    Debug.LogError($"[Generator] Invalid logic type: {info.key}");
                    continue;
                }

                if (!bool.TryParse(info.isMul, out var isMul))
                {
                    isMul = false;
                }

                if (typeMap.TryGetValue(logicType, out var generatorType))
                {
                    result.Add(((INodeLogicGenerator)Activator.CreateInstance(generatorType)).Generate(info.value, isMul));
                }
                else
                {
                    Debug.LogError($"[Generator] Missing generator type for logic type: {info.key}");
                }
                
            }
            
            return result.ToArray();
        }

        public static LogicType GetLogicType(string key)
        {
            if (Enum.TryParse(key, out LogicType logicType))
                return logicType;
            return LogicType.None;
        }
    }

    public struct NodeLogicInfo
    {
        public string key;
        public List<string> value;
        public string isMul;
    }

    public interface INodeLogicGenerator
    {
        public INodeActivateLogic Generate(List<string> value, bool isMul);
    }
    
    public enum LogicType
    {
        None,
        UnlockDash,
        UnlockFlash,
        UnlockCounter,
        UnlockAttackPassive,
        UnlockChargePassive,
        UnlockMovePassive,
        UnlockCounterPassive,
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