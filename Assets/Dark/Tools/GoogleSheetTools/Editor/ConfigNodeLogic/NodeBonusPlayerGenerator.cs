using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusDropRate)]
    public class NodeBonusDropRateGenerator : INodeLogicGenerator
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
                return new NodeBonusDropRate()
                {
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusDropRate value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusBaseDamage)]
    public class NodeBonusBaseDamageGenerator : INodeLogicGenerator
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
                return new NodeBonusPlayer()
                {
                    bonusType = NodeBonusPlayer.BonusPlayerType.Damage,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusBaseDamage value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusBaseCriticalRate)]
    public class NodeBonusBaseCriticalRateGenerator : INodeLogicGenerator
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
                return new NodeBonusPlayer()
                {
                    bonusType = NodeBonusPlayer.BonusPlayerType.CriticalRate,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusBaseCriticalRate value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusBaseCriticalDamage)]
    public class NodeBonusBaseCriticalDamageGenerator : INodeLogicGenerator
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
                return new NodeBonusPlayer()
                {
                    bonusType = NodeBonusPlayer.BonusPlayerType.CriticalDame,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusBaseCriticalDamage value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusBaseCooldown)]
    public class NodeBonusBaseCooldownGenerator : INodeLogicGenerator
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
                return new NodeBonusPlayer()
                {
                    bonusType = NodeBonusPlayer.BonusPlayerType.Cooldown,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusBaseCooldown value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusBaseHp)]
    public class NodeBonusBaseHpGenerator : INodeLogicGenerator
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
                return new NodeBonusPlayer()
                {
                    bonusType = NodeBonusPlayer.BonusPlayerType.Health,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusBaseHp value string: {value[0]}");
            }
        }
    }
}