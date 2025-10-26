using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    [ConfigNodeLogicType(LogicType.UnlockNormalBulletSplit)]
    public class NodeUnlockNormalBulletSplitGenerator : INodeLogicGenerator
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
                return new NodeProjectileActivateAction()
                {
                    actions = bonusAmount.Select((bonusValue) => new ProjectileActivateSplit()
                    {
                        projectile = projectile,
                        amount = bonusValue,
                        angle = 50
                    } as IProjectileActivate).ToList(),
                    isCharge = false
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid UnlockNormalBulletSplit value string: {value[0]}");
            }
        }
    }
}