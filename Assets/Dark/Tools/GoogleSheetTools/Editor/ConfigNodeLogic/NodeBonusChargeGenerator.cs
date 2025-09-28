using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.BonusChargeSize)]
    public class NodeBonusChargeSizeGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var bonusTypeIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }

            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusChargeSize()
                {
                    bonusType = (NodeBonusChargeBullet.BonusType)bonusTypeIndex,
                    value = bonusValue,
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusChargeSize value string: {value[0]}");
            }
        }
    }
    
    [ConfigNodeLogicType(LogicType.BonusChargeBullet)]
    public class NodeBonusChargeBulletGenerator : INodeLogicGenerator
    {
        public INodeActivateLogic Generate(string subType, List<string> value, bool isMul)
        {
            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (!int.TryParse(subType, out var bonusTypeIndex))
            {
                Debug.LogError($"Invalid sub-type string: {subType}");
                return null;
            }

            try
            {
                var bonusValue = value[0].Split(',').Select((str) => float.Parse(str, CultureInfo.InvariantCulture)).ToArray();
                return new NodeBonusChargeBullet()
                {
                    bonusType = (NodeBonusChargeBullet.BonusType)bonusTypeIndex,
                    value = bonusValue,
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid BonusChargeBullet value string: {value[0]}");
            }
        }
    }
}