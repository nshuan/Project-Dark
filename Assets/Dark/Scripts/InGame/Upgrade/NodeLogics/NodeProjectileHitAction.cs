using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeProjectileHitAction : INodeActivateLogic
    {
        [OdinSerialize, NonSerialized] private List<IProjectileHit> actions;
        [SerializeField] private bool isCharge;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (actions  == null) return;
            
            if (isCharge)
            {
                bonusInfo.skillBonus.projectileChargeHitActions ??= new List<IProjectileHit>();
                foreach (var action in actions)
                {
                    if (bonusInfo.skillBonus.projectileChargeHitActions.Any((a) => a.GetType() == action.GetType())) continue;
                    bonusInfo.skillBonus.projectileChargeHitActions.Add(action);
                }
            }
            else
            {
                bonusInfo.skillBonus.projectileHitActions ??= new List<IProjectileHit>();
                foreach (var action in actions)
                {
                    if (bonusInfo.skillBonus.projectileHitActions.Any((a) => a.GetType() == action.GetType())) continue;
                    bonusInfo.skillBonus.projectileHitActions.Add(action);
                }
            }
        }
    }
}