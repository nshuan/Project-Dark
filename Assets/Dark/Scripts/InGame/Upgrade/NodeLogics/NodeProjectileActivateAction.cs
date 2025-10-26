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
        [OdinSerialize, NonSerialized] public List<IProjectileActivate> actions;
        public bool isCharge;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (actions == null) return;
            if (level <= 0 || level > actions.Count) return;
            
            var action = actions[level - 1];
            
            if (isCharge)
            {
                bonusInfo.skillBonus.projectileChargeActivateActions ??= new List<IProjectileActivate>();
                
                var exist = bonusInfo.skillBonus.projectileChargeActivateActions.FirstOrDefault((a) =>
                    a.GetType() == action.GetType());
                if (exist == null)
                    bonusInfo.skillBonus.projectileChargeActivateActions.Add(action);
                else
                    exist.Combine(action);
            }
            else
            {
                bonusInfo.skillBonus.projectileActivateActions ??= new List<IProjectileActivate>();
                var exist = bonusInfo.skillBonus.projectileActivateActions.FirstOrDefault((a) =>
                    a.GetType() == action.GetType());
                if (exist == null)
                    bonusInfo.skillBonus.projectileActivateActions.Add(action);
                else
                    exist.Combine(action);
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= actions.Count) level = actions.Count - 1;
            return actions[level].GetValue().ToString();
        }

        public int MaxLevel => 1;
    }
}