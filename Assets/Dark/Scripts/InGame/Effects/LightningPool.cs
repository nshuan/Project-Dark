using Core;
using UnityEngine;

namespace InGame.Effects
{
    public class LightningPool : Pool<Lightning, LightningPool>
    {
        public override Lightning Get(Transform targetParent, bool active = true)
        {
            var obj = base.Get(targetParent, active);
            obj.Init();
            return obj;
        }
    }
}