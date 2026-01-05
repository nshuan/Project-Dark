using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dark.Tools.GoogleSheetTool;
using InGame.Upgrade.DynamicBonus;
using UnityEngine;

namespace InGame.Upgrade.NodeLogicsV2
{
    [Serializable]
    [ConfigNodeLogicTypeV2(NodeBonusTypeV2.BonusBaseCooldown)]
    public class NodeV2BonusBaseCooldown : INodeActivateLogicV2, INodeLogicGeneratorV2, INodeDynamicBonusValueV2
    {
        public float[] value;
        public bool isMul;
        public bool isDynamic;
        
        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (value == null || level <= 0 || level > value.Length) return;
            if (bonusInfo == null) return;
            
            if (isMul) bonusInfo.bonusBase.bonusCooldown.mul += value[level - 1];
            else bonusInfo.bonusBase.bonusCooldown.add += value[level - 1];
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            var before = "";

            before = LevelUtilityV2.GetBaseCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture) + "s";

            if (level > value.Length)
                return (before, before);
 
            var cooldownMultiply = bonusInfo.bonusBase.bonusCooldown.mul;
            var cooldownPlus = bonusInfo.bonusBase.bonusCooldown.add;

            ActivateNode(level, ref bonusInfo);
            
            var after = "";
            after = LevelUtilityV2.GetBaseCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture) + "s";
            
            bonusInfo.bonusBase.bonusCooldown.mul = cooldownMultiply;
            bonusInfo.bonusBase.bonusCooldown.add = cooldownPlus;
            
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            if (isMul) return (value[level] * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
            else return value[level].ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value?.Length ?? 0;
        public INodeActivateLogicV2 Generate(string subType, List<string> listValue, bool mul)
        {
            // Trống bonus value thì lấy dynamic
            if (listValue == null || listValue.Count == 0)
            {
                isMul = mul;
                isDynamic = true;
                return null;
            }

            try
            {
                var bonusValue = listValue[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture))
                    .ToArray();
                value = bonusValue;
                isMul = mul;

                return this;
            }
            catch (Exception e)
            {
                Debug.LogError($"Invalid BonusBaseCooldown value string: {listValue[0]}");
                value = Array.Empty<float>();
                isMul = mul;
                return this;
            }
        }

        public bool IsDynamic => isDynamic;
        public void OverrideBonusValue(int groupUnlockOrder)
        {
            if (MaxLevel == 1)
            {
                var dynamicValue = DynamicBonusValueConfig.Instance.GetBonus1Stage(NodeBonusTypeV2.BonusBaseCooldown, groupUnlockOrder);
                value = new[] { dynamicValue };
            }
            else
            {
                value = DynamicBonusValueConfig.Instance.GetBonus5Stage(NodeBonusTypeV2.BonusBaseCooldown, groupUnlockOrder).ToArray();
            }
        }
    }
}

