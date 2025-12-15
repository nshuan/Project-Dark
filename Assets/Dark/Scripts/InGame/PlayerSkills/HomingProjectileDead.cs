using System;
using UnityEngine;

namespace InGame
{
    public class HomingProjectileDead : ProjectileDead
    {
        [SerializeField] private GameObject vfxDispose;
        public Transform visual;
        public float disposeDuration = 1f;

        public float Dispose()
        {
            visual.gameObject.SetActive(false);
            vfxDispose.SetActive(true);
            return disposeDuration;
        }

        public void Reset()
        {
            visual.gameObject.SetActive(true);
            vfxDispose.SetActive(false);
        }

        protected override void Swallowed()
        {
            Dispose();
            ProjectileHomingDeadPool.Instance.Release(this);
        }
    }
}