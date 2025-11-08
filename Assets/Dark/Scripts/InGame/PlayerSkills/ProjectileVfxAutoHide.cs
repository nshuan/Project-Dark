using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class ProjectileVfxAutoHide : MonoBehaviour
    {
        public float duration = 1f;

        public void Activate(Action<ProjectileVfxAutoHide> completeAction)
        {
            transform.SetParent(null);
            StartCoroutine(IEActivate(completeAction));
        }

        private IEnumerator IEActivate(Action<ProjectileVfxAutoHide> completeAction)
        {
            yield return new WaitForSeconds(duration);
            completeAction?.Invoke(this);
        }
    }
}