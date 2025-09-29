using System.Collections.Generic;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.UnlockAttackPassive)]
    public class NodeUnlockAttackPassiveGenerator :  INodeLogicGenerator
    {
        /// <summary>
        /// subType: passive ID
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

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }

            return new NodeUnlockPassive()
            {
                triggerType = PassiveTriggerType.DameByNormalAttack,
                passiveType = (PassiveType)passiveIndex,
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockChargePassive)]
    public class NodeUnlockChargePassiveGenerator :  INodeLogicGenerator
    {
        /// <summary>
        /// subType: passive ID
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

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid passive sub-type string: {subType}");
                return null;
            }

            return new NodeUnlockPassive()
            {
                triggerType = PassiveTriggerType.DameByChargeAttack,
                passiveType = (PassiveType)passiveIndex,
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockMovePassive)]
    public class NodeUnlockMovePassiveGenerator :  INodeLogicGenerator
    {
        /// <summary>
        /// subType: passive ID
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

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid passive sub-type string: {subType}");
                return null;
            }

            return new NodeUnlockPassive()
            {
                triggerType = PassiveTriggerType.DameByMoveSKill,
                passiveType = (PassiveType)passiveIndex,
            };
        }
    }
    
    [ConfigNodeLogicType(LogicType.UnlockCounterPassive)]
    public class NodeUnlockCounterPassiveGenerator :  INodeLogicGenerator
    {
        /// <summary>
        /// subType: passive ID
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

            if (!int.TryParse(subType, out var passiveIndex))
            {
                Debug.LogError($"Invalid passive sub-type string: {subType}");
                return null;
            }

            return new NodeUnlockPassive()
            {
                triggerType = PassiveTriggerType.TowerTakeDame,
                passiveType = (PassiveType)passiveIndex,
            };
        }
    }
}