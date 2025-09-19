using System;
using System.Collections;
using Dark.Scripts.Utils;
using Economic;
using UnityEngine;

namespace InGame.Economic.DropItems
{
    public class EItemDrop : MonoBehaviour
    {
        private const float RotateSpeed = 250f;
        private const float FlySpeed = 5f;
        
        [SerializeField] private GameObject vfxClaim;
        [SerializeField] private GameObject visual;
        
        [Space]
        [Header("Config")]
        [SerializeField] private AnimationCurve flySpeedCurve;
        [SerializeField] private float timeToReachMaxSpeed = 1f;
        
        public WealthType kind;
        public int Quantity { get; set; }
        
        public void Drop()
        {
            visual.gameObject.SetActive(true);
        }

        public void Collect(Transform target, Action onCompleteVfx)
        {
            StartCoroutine(IECollect(target, 1f, () =>
            {
                visual.gameObject.SetActive(false);
                vfxClaim.SetActive(true);
            }, () =>
            {
                vfxClaim.SetActive(false);
                onCompleteVfx?.Invoke();
            }));
        }

        private IEnumerator IECollect(Transform target, float delayComplete, Action onReachTarget, Action onComplete)
        {
            var direction = target.position - transform.position;
            var lifeTime = 0f;

            while (Vector3.Distance(transform.position, target.position) > 1f)
            {
                direction = Vector3.RotateTowards(direction, target.position - transform.position,
                    Mathf.Deg2Rad * RotateSpeed * Time.deltaTime, 0f);
                transform.position += FlySpeed * (lifeTime < timeToReachMaxSpeed ? flySpeedCurve.Evaluate(lifeTime / timeToReachMaxSpeed) : 1f)
                                               * Time.deltaTime * direction;
                lifeTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            onReachTarget?.Invoke();
            yield return new WaitForSeconds(delayComplete);
            onComplete?.Invoke();
        }
    }
}