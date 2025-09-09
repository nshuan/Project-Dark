using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusPassiveDamage)]
    public class NodeBonusPassiveDamageGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }
            
            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusPassive()
                {
                    bonusType = NodeBonusPassive.BonusType.Damage,
                    passiveType = (PassiveType)passiveIndex,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusPassiveDamage value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusPassiveSize)]
    public class NodeBonusPassiveSizeGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }
            
            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusPassive()
                {
                    bonusType = NodeBonusPassive.BonusType.Size,
                    passiveType = (PassiveType)passiveIndex,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusPassiveSize value string: {value[0]}");
            }
        }
    }
}