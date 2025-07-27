using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeProjectileActivateAction : INodeActivateLogic
    {
        [OdinSerialize, NonSerialized] private List<IProjectileActivate> actions;
        [SerializeField] private bool isCharge;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (actions  == null) return;
            
            if (isCharge)
            {
                bonusInfo.skillBonus.projectileChargeActivateActions ??= new List<IProjectileActivate>();
                foreach (var action in actions)
                {
                    if (bonusInfo.skillBonus.projectileChargeActivateActions.Any((a) => a.GetType() == action.GetType())) continue;
                    bonusInfo.skillBonus.projectileChargeActivateActions.Add(action);
                }
            }
            else
            {
                bonusInfo.skillBonus.projectileActivateActions ??= new List<IProjectileActivate>();
                foreach (var action in actions)
                {
                    if (bonusInfo.skillBonus.projectileActivateActions.Any((a) => a.GetType() == action.GetType())) continue;
                    bonusInfo.skillBonus.projectileActivateActions.Add(action);
                }
            }
        }
    }
}