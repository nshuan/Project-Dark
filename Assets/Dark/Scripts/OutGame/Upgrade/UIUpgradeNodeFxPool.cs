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
        [SerializeField] private UIParticle vfxAppear;

        protected Queue<UIParticle> poolUnlock = new();
        protected Queue<UIParticle> poolActivate = new();
        protected Queue<UIParticle> poolActivateMax = new();
        protected Queue<UIParticle> poolAppear = new();
        
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
            obj.transform.localPosition = Vector3.zero;
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
            obj.transform.localPosition = Vector3.zero;
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
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(active);
            return obj;
        }

        public UIParticle GetVfxAppear(Transform targetParent, bool active = true)
        {
            if (poolAppear.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.transform.localPosition = Vector3.zero;
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            obj = Instantiate(vfxAppear, targetParent);
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(active);
            return obj;
        }

        public void ReleaseVfxUnlock(UIParticle vfx)
        {
            vfx.transform.SetParent(transform);
            vfx.gameObject.SetActive(false);
            if (poolUnlock.Contains(vfx)) return;
            poolUnlock.Enqueue(vfx);
        }

        public void ReleaseVfxActivate(UIParticle vfx)
        {
            vfx.transform.SetParent(transform);
            vfx.gameObject.SetActive(false);
            if (poolActivate.Contains(vfx)) return;
            poolActivate.Enqueue(vfx);
        }

        public void ReleaseVfxActivateMax(UIParticle vfx)
        {
            vfx.transform.SetParent(transform);
            vfx.gameObject.SetActive(false);
            if (poolActivateMax.Contains(vfx)) return;
            poolActivateMax.Enqueue(vfx);
        }
        
        public void ReleaseVfxAppear(UIParticle vfx)
        {
            vfx.transform.SetParent(transform);
            vfx.gameObject.SetActive(false);
            if (poolAppear.Contains(vfx)) return;
            poolAppear.Enqueue(vfx);
        }
    }
}