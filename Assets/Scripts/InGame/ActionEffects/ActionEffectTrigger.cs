using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public abstract class ActionEffectTrigger : MonoBehaviour
    {
        public virtual void Setup(List<ActionEffectConfig> effectConfigs)
        {
            if (effectConfigs == null) return;
            foreach (var effectCfg in effectConfigs)
            {
                Setup(effectCfg);
            }
        }

        protected abstract void Setup(ActionEffectConfig effectConfig);
    }
}