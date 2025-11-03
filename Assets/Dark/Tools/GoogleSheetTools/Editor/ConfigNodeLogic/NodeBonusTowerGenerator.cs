using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusHpRegenPerSec)]
    public class NodeBonusHpRegenPerSecGenerator : INodeLogicGenerator
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
                return new NodeBonusHpRegen()
                {
                    bonusType = NodeBonusHpRegen.BonusType.AutoRegenerate,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusHpRegenPerSec value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusHpRegenOnKill)]
    public class NodeBonusHpRegenOnKillGenerator : INodeLogicGenerator
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
                return new NodeBonusHpRegen()
                {
                    bonusType = NodeBonusHpRegen.BonusType.OnEnemyDied,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusHpRegenOnKill value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusCounterCooldown)]
    public class NodeBonusCounterCooldownGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var counterType) || counterType < 0 || counterType >= Enum.GetValues(typeof(NodeTowerCounter.CounterType)).Length)
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }
            
            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeTowerCounterBonus()
                {
                    counterType = (NodeTowerCounter.CounterType)counterType,
                    bonusType = NodeTowerCounterBonus.BonusType.Cooldown,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusCounterCooldown value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusCounterDamage)]
    public class NodeBonusCounterDamageGenerator : INodeLogicGenerator
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
                return new NodeTowerCounterBonus()
                {
                    bonusType = NodeTowerCounterBonus.BonusType.Damage,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusCounterDamage value string: {value[0]}");
            }
        }
    }
}