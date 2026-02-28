using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeLogicFactoryV2
    {
        private static Dictionary<NodeBonusTypeV2, Type> typeMap;

        static ConfigNodeLogicFactoryV2()
        {
            GenerateTypeMap();
        }

        private static void GenerateTypeMap()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(ConfigNodeLogicTypeV2Attribute), false).Any())
                .ToList();
            
            var dupes = types.GroupBy(t => ((ConfigNodeLogicTypeV2Attribute)t
                .GetCustomAttributes(typeof(ConfigNodeLogicTypeV2Attribute), false)
                .First()).LogicType).Where(g => g.Count() > 1).ToList();

            if (dupes.Count > 0)
            {
                typeMap = null;
                throw new Exception("[Factory] There are more than 1 generator with the same logic type: \n" +
                                    string.Join(Environment.NewLine, dupes.Select(g => g.Key)));
            }
            
            typeMap = types.ToDictionary(
                t => ((ConfigNodeLogicTypeV2Attribute)t
                    .GetCustomAttributes(typeof(ConfigNodeLogicTypeV2Attribute), false)
                    .First()).LogicType,
                t => t
            );
        }
        
        public static INodeActivateLogicV2[] Generate(List<NodeLogicInfo> infos)
        {
            if (typeMap == null) GenerateTypeMap();
            
            var result = new List<INodeActivateLogicV2>();

            foreach (var info in infos)
            {
                var logicType = GetLogicType(info.key);
                if (logicType == NodeBonusTypeV2.None)
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
                    result.Add(((INodeLogicGeneratorV2)Activator.CreateInstance(generatorType)).Generate(info.value[0], info.value.GetRange(0, info.value.Count), isMul));
                }
                else
                {
                    Debug.LogError($"[Generator] Missing generator type for logic type: {info.key}");
                }
                
            }
            
            return result.ToArray();
        }

        public static NodeBonusTypeV2 GetLogicType(string key)
        {
            if (Enum.TryParse(key, out NodeBonusTypeV2 logicType))
                return logicType;
            return NodeBonusTypeV2.None;
        }
    }

    /// <summary>
    /// Key = Logic type key, should match the key in enum LogicType
    /// value = values to set, 1st item should be the subtype of logic. If there isn't, it should be an empty string
    /// </summary>
    public struct NodeLogicInfo
    {
        public string key;
        public List<string> value;
        public string isMul;
    }
    
    public interface INodeLogicGeneratorV2
    {
        public INodeActivateLogicV2 Generate(string subType, List<string> listValue, bool mul);
    }
    
    public enum NodeBonusTypeV2
    {
        None,
        // Unlock
        UnlockNormalAttackPiercing,
        UnlockNormalAttackBullet,
        UnlockChargeAttackBullet,
        UnlockChargeAttackSize,
        UnlockMoveFlash,
        UnlockMoveDash,
        UnlockCounterPiercing,
        UnlockCounterSlash,
        UnlockPassiveNormalLightning,
        UnlockPassiveNormalExplosive,
        UnlockPassiveNormalBurning,
        UnlockPassiveNormalThunder,
        UnlockPassiveChargeLightning,
        UnlockPassiveChargeExplosive,
        UnlockPassiveChargeBurning,
        UnlockPassiveChargeThunder,
        UnlockPassiveMoveLightning,
        UnlockPassiveMoveExplosive,
        UnlockPassiveMoveBurning,
        UnlockPassiveMoveThunder,
        UnlockPassiveCounterLightning,
        UnlockPassiveCounterExplosive,
        UnlockPassiveCounterBurning,
        UnlockPassiveCounterThunder,
        // Base bonuses
        BonusBaseHp,
        BonusBaseShield,
        BonusBaseDmg,
        BonusBaseCooldown,
        BonusBaseRegen,
        BonusBaseLifeLeech,
        BonusBaseCritDmg,
        BonusBaseCritRate,
        BonusBaseStagger,
        BonusBaseDmgBoss,
        BonusBaseVestigeDrop,
        BonusBaseVestigeDoubleChance,
        BonusBaseVestigeTripleChance,
        BonusBaseVestigeCollectSize,
        BonusBaseExpDrop,
        // Normal Attack bonuses
        BonusNormalAttackDmg,
        BonusNormalAttackCooldown,
        BonusNormalAttackRange,
        BonusPiercingDmg,
        BonusPiercingAmount,
        BonusBulletDmg,
        BonusBulletAmount,
        // Charge Attack bonuses
        BonusChargeCooldown,
        BonusChargeTime,
        BonusChargeDmg,
        BonusChargeBulletAmount,
        BonusChargeSizeAmount,
        BonusChargeRangeStep,
        // Move bonuses
        BonusMoveDmg,
        BonusMoveCooldown,
        BonusFlashSize,
        BonusDashSize,
        // Counter bonuses
        BonusCounterDmg,
        BonusCounterCooldown,
        BonusCounterPiercingAmount,
        BonusCounterSlashSize,
        // Passive Normal Attack bonuses
        BonusPassiveNormalLightningDmg,
        BonusPassiveNormalLightningRate,
        BonusPassiveNormalLightningAmount,
        BonusPassiveNormalExplosiveDmg,
        BonusPassiveNormalExplosiveRate,
        BonusPassiveNormalExplosiveSize,
        BonusPassiveNormalBurningDmg,
        BonusPassiveNormalBurningRate,
        BonusPassiveNormalBurningDuration,
        BonusPassiveNormalThunderDmg,
        BonusPassiveNormalThunderRate,
        BonusPassiveNormalThunderExecutionChance,
        // Passive Charge Attack bonuses
        BonusPassiveChargeLightningDmg,
        BonusPassiveChargeLightningRate,
        BonusPassiveChargeLightningAmount,
        BonusPassiveChargeExplosiveDmg,
        BonusPassiveChargeExplosiveRate,
        BonusPassiveChargeExplosiveSize,
        BonusPassiveChargeBurningDmg,
        BonusPassiveChargeBurningRate,
        BonusPassiveChargeBurningDuration,
        BonusPassiveChargeThunderDmg,
        BonusPassiveChargeThunderRate,
        BonusPassiveChargeThunderExecutionChance,
        // Passive Move bonuses
        BonusPassiveMoveLightningDmg,
        BonusPassiveMoveLightningRate,
        BonusPassiveMoveLightningAmount,
        BonusPassiveMoveExplosiveDmg,
        BonusPassiveMoveExplosiveRate,
        BonusPassiveMoveExplosiveSize,
        BonusPassiveMoveBurningDmg,
        BonusPassiveMoveBurningRate,
        BonusPassiveMoveBurningDuration,
        BonusPassiveMoveThunderDmg,
        BonusPassiveMoveThunderRate,
        BonusPassiveMoveThunderExecutionChance,
        // Passive Counter bonuses
        BonusPassiveCounterLightningDmg,
        BonusPassiveCounterLightningRate,
        BonusPassiveCounterLightningAmount,
        BonusPassiveCounterExplosiveDmg,
        BonusPassiveCounterExplosiveRate,
        BonusPassiveCounterExplosiveSize,
        BonusPassiveCounterBurningDmg,
        BonusPassiveCounterBurningRate,
        BonusPassiveCounterBurningDuration,
        BonusPassiveCounterThunderDmg,
        BonusPassiveCounterThunderRate,
        BonusPassiveCounterThunderExecutionChance,
    }
}