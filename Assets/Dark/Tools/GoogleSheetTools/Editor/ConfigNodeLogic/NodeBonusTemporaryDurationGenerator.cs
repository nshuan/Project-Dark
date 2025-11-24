using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.TempBonusAttackSpeedDuration)]
    public class NodeTempBonusAttackSpeedDurationGenerator : INodeLogicGenerator
    {
        /// <summary>
        /// subType: bonus trigger type
        /// 0 = on enemy killed
        /// 1 = on move
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isMul"></param>
        /// <returns></returns>
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var bonusTypeIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }

            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusTemporaryDuration()
                {
                    bonusType = bonusTypeIndex == 0 ? NodeBonusTemporary.BonusTemporaryType.AtkSpeOnKill : NodeBonusTemporary.BonusTemporaryType.AtkSpeOnMove,
                    value = bonusValue,
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid TempBonusAttackSpeedDuration value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.TempBonusDamageDuration)]
    public class NodeTempBonusDamageDurationGenerator : INodeLogicGenerator
    {
        /// <summary>
        /// subType: bonus trigger type
        /// 0 = on enemy killed
        /// 1 = on move
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isMul"></param>
        /// <returns></returns>
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var bonusTypeIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }

            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture))
                    .ToArray();
                return new NodeBonusTemporaryDuration()
                {
                    bonusType = bonusTypeIndex == 0
                        ? NodeBonusTemporary.BonusTemporaryType.DamageOnKill
                        : NodeBonusTemporary.BonusTemporaryType.DamageOnMove,
                    value = bonusValue,
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid TempBonusDamageDuration value string: {value[0]}");
            }
        }
    }
}