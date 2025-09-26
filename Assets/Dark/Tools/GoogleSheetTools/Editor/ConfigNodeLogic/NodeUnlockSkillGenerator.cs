using System.Collections.Generic;
using InGame;
using InGame.Upgrade;

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
            return new NodeTowerCounter()
            {
                
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
    
    [ConfigNodeLogicType(LogicType.UnlockSplitBullet)]
    public class NodeLogicUnlockSplitBulletGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeProjectileActivateAction()
            {
                actions = new List<IProjectileActivate>() { new ProjectileActivateSplit() { amount = 2, angle = 56f } }
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockBulletHitBlossom)]
    public class NodeLogicUnlockBulletHitBlossomGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            return new NodeProjectileHitAction()
            {
                actions = new List<IProjectileHit>() { new ProjectileHitBlossom() { bulletAmount = 5, blossomSize = 3f, } }
            };
        }
    }
}