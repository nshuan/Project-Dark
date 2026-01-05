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
    [ConfigNodeLogicTypeV2(NodeBonusTypeV2.BonusBaseHp)]
    public class NodeV2BonusBaseHp : INodeActivateLogicV2, INodeLogicGeneratorV2, INodeDynamicBonusValueV2
    {
        public float[] value;
        public bool isMul;
        public bool isDynamic;
        
        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (value == null || level <= 0 || level > value.Length) return;
            if (bonusInfo == null) return;
            
            if (isMul) bonusInfo.bonusBase.bonusHp.mul += value[level - 1];
            else bonusInfo.bonusBase.bonusHp.add += value[level - 1];
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            var before = "";

            before = LevelUtilityV2.GetBaseTowerHp().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);

            if (level > value.Length)
                return (before, before);
 
            var hpMultiply = bonusInfo.bonusBase.bonusHp.mul;
            var hpPlus = bonusInfo.bonusBase.bonusHp.add;

            ActivateNode(level, ref bonusInfo);
            
            var after = "";
            after = LevelUtilityV2.GetBaseTowerHp().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
            
            bonusInfo.bonusBase.bonusHp.mul = hpMultiply;
            bonusInfo.bonusBase.bonusHp.add = hpPlus;
            
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
                Debug.LogError($"Invalid BonusBaseHp value string: {listValue[0]}");
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
                var dynamicValue = DynamicBonusValueConfig.Instance.GetBonus1Stage(NodeBonusTypeV2.BonusBaseHp, groupUnlockOrder);
                value = new[] { dynamicValue };
            }
            else
            {
                value = DynamicBonusValueConfig.Instance.GetBonus5Stage(NodeBonusTypeV2.BonusBaseHp, groupUnlockOrder).ToArray();
            }
        }
    }
}