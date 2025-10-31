using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    
    [ConfigNodeLogicType(LogicType.UnlockNormalAtkSpe)]
    public class NodeUnlockNormalAtkSpeGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var projectileId))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }
            
            try
            {
                var bonusAmount = value[0].Split(',').Select((str) => int.Parse(str, CultureInfo.InvariantCulture)).ToArray();
#if UNITY_EDITOR
                var projectile = ProjectileManifest.EditorGet(projectileId);
#endif
                return new NodeUnlockNormalAtkSpe()
                {
                    action = new NodeProjectileActivateAction()
                    {
                        actions = bonusAmount.Select((bonusValue) => new ProjectileActivateSplit()
                        {
                            projectile = projectile,
                            amount = bonusValue,
                            angle = 50
                        } as IProjectileActivate).ToList(),
                        isCharge = false
                    }
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid UnlockNormalAtkSpe value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockNormalDame)]
    public class NodeUnlockNormalDameGenerator : INodeLogicGenerator
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
                return new NodeUnlockNormalDame()
                {
                    bonus = new NodeBonusSkill()
                    {
                        bonusType = NodeBonusSkill.BonusSkillType.BulletMaxHit,
                        value = bonusValue,
                        isMultiply = isMul
                    }
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid UnlockNormalDame value string: {value[0]}");
            }
        }
    }
}