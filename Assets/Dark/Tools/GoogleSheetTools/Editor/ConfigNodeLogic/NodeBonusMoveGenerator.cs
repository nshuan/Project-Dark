using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusMoveCooldown)]
    public class NodeBonusMoveCooldownGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusMoveTower()
                {
                    bonusType = NodeBonusMoveTower.BonusMoveTowerType.Cooldown,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusMoveCooldown value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusMoveCastTime)]
    public class NodeBonusMoveCastTimeGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeBonusMoveTower()
            {
                bonusType = NodeBonusMoveTower.BonusMoveTowerType.CastTime,
                isMultiply = isMul
            };
        }
    }
}