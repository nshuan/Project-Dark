using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dark.Tools.GoogleSheetTool;

namespace InGame.Upgrade.NodeLogicsV2
{
    [Serializable]
    [ConfigNodeLogicTypeV2(NodeBonusTypeV2.BonusPassiveCounterThunderDmg)]
    public class NodeV2BonusPassiveCounterThunderDmg : INodeActivateLogicV2, INodeLogicGeneratorV2
    {
        public float[] value;
        public bool isMul;
        
        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (value == null || level <= 0 || level > value.Length) return;
            if (bonusInfo == null) return;
            
            if (isMul) bonusInfo.bonusPassiveCounter.bonusThunderDmg.mul += value[level - 1];
            else bonusInfo.bonusPassiveCounter.bonusThunderDmg.add += value[level - 1];
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            var before = "";
            before = LevelUtilityV2.GetPassiveValue(PassiveTriggerType.TowerTakeDame, PassiveType.Thunder).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);

            if (level > value.Length)
                return (before, before);
 
            var thunderDmgMultiply = bonusInfo.bonusPassiveCounter.bonusThunderDmg.mul;
            var thunderDmgPlus = bonusInfo.bonusPassiveCounter.bonusThunderDmg.add;

            ActivateNode(level, ref bonusInfo);
            
            var after = "";
            after = LevelUtilityV2.GetPassiveValue(PassiveTriggerType.TowerTakeDame, PassiveType.Thunder).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
            
            bonusInfo.bonusPassiveCounter.bonusThunderDmg.mul = thunderDmgMultiply;
            bonusInfo.bonusPassiveCounter.bonusThunderDmg.add = thunderDmgPlus;
            
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
            if (listValue == null || listValue.Count == 0)
            {
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
                throw new Exception($"Invalid BonusPassiveCounterThunderDmg value string: {listValue[0]}");
            }
        }
    }
}


