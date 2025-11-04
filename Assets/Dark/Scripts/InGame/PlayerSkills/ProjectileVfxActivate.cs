using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class ProjectileVfxActivate : MonoBehaviour
    {
        public float duration = 1f;

        public void Activate()
        {
            transform.SetParent(null);
            StartCoroutine(IEActivate());
        }

        private IEnumerator IEActivate()
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }
    }
}