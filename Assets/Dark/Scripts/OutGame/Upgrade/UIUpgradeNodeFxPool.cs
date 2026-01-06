using System.Collections.Generic;
using Coffee.UIExtensions;
using Core;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeFxPool<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {
        [SerializeField] private UIParticle vfxUnlock;
        [SerializeField] private UIParticle vfxActivate;
        [SerializeField] private UIParticle vfxActivateMax;

        protected Queue<UIParticle> poolUnlock = new();
        protected Queue<UIParticle> poolActivate = new();
        protected Queue<UIParticle> poolActivateMax = new();
        
        public UIParticle GetVfxUnlock(Transform targetParent, bool active = true)
        {
            if (poolUnlock.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.transform.localPosition = Vector3.zero;
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            obj = Instantiate(vfxUnlock, targetParent);
            obj.gameObject.SetActive(active);
            return obj;
        }

        public UIParticle GetVfxActivate(Transform targetParent, bool active = true)
        {
            if (poolActivate.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.transform.localPosition = Vector3.zero;
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            obj = Instantiate(vfxActivate, targetParent);
            obj.gameObject.SetActive(active);
            return obj;
        }

        public UIParticle GetVfxActivateMax(Transform targetParent, bool active = true)
        {
            if (poolActivateMax.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.transform.localPosition = Vector3.zero;
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            obj = Instantiate(vfxActivateMax, targetParent);
            obj.gameObject.SetActive(active);
            return obj;
        }

        public void ReleaseVfxUnlock(UIParticle vfxUnlock)
        {
            vfxUnlock.transform.SetParent(transform);
            vfxUnlock.gameObject.SetActive(false);
            if (poolUnlock.Contains(vfxUnlock)) return;
            poolUnlock.Enqueue(vfxUnlock);
        }

        public void ReleaseVfxActivate(UIParticle vfxActivate)
        {
            vfxActivate.transform.SetParent(transform);
            vfxActivate.gameObject.SetActive(false);
            if (poolActivate.Contains(vfxActivate)) return;
            poolActivate.Enqueue(vfxActivate);
        }

        public void ReleaseVfxActivateMax(UIParticle vfxActivateMax)
        {
            vfxActivateMax.transform.SetParent(transform);
            vfxActivateMax.gameObject.SetActive(false);
            if (poolActivateMax.Contains(vfxActivateMax)) return;
            poolActivateMax.Enqueue(vfxActivateMax);
        }
    }
}