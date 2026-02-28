using System.Collections;
using UnityEngine;

namespace InGame
{
    public class KnightAutoAimProjectile : AutoAimProjectileEntity
    {
        [SerializeField] private GameObject vfxSlash;
        [SerializeField] private float delayShowVfx = 0.02f;
        
        protected override IEnumerator IEActivate(float delay)
        {
            Size = 1f;
            transform.localScale = Vector3.one;
            vfxSlash.SetActive(false);

            yield return StartCoroutine(base.IEActivate(delay));

            yield return new WaitForSeconds(delayShowVfx);
            vfxSlash.SetActive(true);
        }
    }
}