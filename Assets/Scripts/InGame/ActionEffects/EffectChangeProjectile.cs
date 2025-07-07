using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class EffectChangeProjectile : IActionEffectLogic
    {
        public ProjectileEntity newProjectile;
        
        public void Initialize()
        {
            
        }

        public void TriggerEffect(int effectId, Vector2 center, float size, float value, float stagger)
        {
            
        }
    }
}