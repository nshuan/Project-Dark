using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame.Upgrade;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusSkillDamage)]
    public class NodeBonusSkillDamageGenerator : INodeLogicGenerator
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
                return new NodeBonusSkill()
                {
                    bonusType = NodeBonusSkill.BonusSkillType.Damage,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusSkillDamage value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusSkillBps)]
    public class NodeBonusSkillBpsGenerator : INodeLogicGenerator
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
                return new NodeBonusSkill()
                {
                    bonusType = NodeBonusSkill.BonusSkillType.BulletNum,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusSkillBps value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusSkillAttackSize)]
    public class NodeBonusSkillAttackSizeGenerator : INodeLogicGenerator
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
                return new NodeBonusSkill()
                {
                    bonusType = NodeBonusSkill.BonusSkillType.Size,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusSkillAttackSize value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusSkillStagger)]
    public class NodeBonusSkillStaggerGenerator : INodeLogicGenerator
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
                return new NodeBonusSkill()
                {
                    bonusType = NodeBonusSkill.BonusSkillType.Stagger,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusSkillStagger value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusSkillBulletMaxHit)]
    public class NodeBonusSkillBulletMaxHitGenerator : INodeLogicGenerator
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
                return new NodeBonusSkill()
                {
                    bonusType = NodeBonusSkill.BonusSkillType.BulletMaxHit,
                    value = bonusValue,
                    isMultiply = isMul
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusSkillBulletMaxHit value string: {value[0]}");
            }
        }
    }
}