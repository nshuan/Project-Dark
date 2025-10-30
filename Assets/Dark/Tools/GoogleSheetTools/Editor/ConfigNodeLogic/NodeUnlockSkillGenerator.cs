using System;
using System.Collections.Generic;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.UnlockDash)]
    public class NodeLogicUnlockDashGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeUnlockSkill()
            {
                unlockType = NodeUnlockSkill.BonusUnlockSkillType.MoveDash
            };
        }
    }

    [ConfigNodeLogicType(LogicType.UnlockFlash)]
    public class NodeLogicUnlockFlashGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeUnlockSkill()
            {
                unlockType = NodeUnlockSkill.BonusUnlockSkillType.MoveFlash
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockCounter)]
    public class NodeLogicUnlockCounterGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (!int.TryParse(subType, out var counterType) || counterType < 0 || counterType >= Enum.GetValues(typeof(NodeTowerCounter.CounterType)).Length)
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }
            
            return new NodeTowerCounter()
            {
                counterType = (NodeTowerCounter.CounterType)counterType,
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockChargeSize)]
    public class NodeLogicUnlockChargeSizeGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeUnlockSkill()
            {
                unlockType = NodeUnlockSkill.BonusUnlockSkillType.ChargeSize
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockChargeBullet)]
    public class NodeLogicUnlockChargeBulletGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeUnlockSkill()
            {
                unlockType = NodeUnlockSkill.BonusUnlockSkillType.ChargeBullet
            };
        }
    }
}