using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusFlashCooldown)]
    public class NodeBonusFlashCooldownGenerator : INodeLogicGenerator
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
                return new NodeBonusFlash()
                {
                    bonusType = NodeBonusFlash.BonusType.Cooldown,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusFlashCooldown value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusFlashSize)]
    public class NodeBonusFlashSizeGenerator : INodeLogicGenerator
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
                return new NodeBonusFlash()
                {
                    bonusType = NodeBonusFlash.BonusType.Size,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusFlashSize value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusFlashDamage)]
    public class NodeBonusFlashDamageGenerator : INodeLogicGenerator
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
                return new NodeBonusFlash()
                {
                    bonusType = NodeBonusFlash.BonusType.Damage,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusFlashDamage value string: {value[0]}");
            }
        }
    }
}