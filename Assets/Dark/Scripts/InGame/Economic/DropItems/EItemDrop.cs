using System;
using System.Collections;
using Dark.Scripts.Utils;
using DG.Tweening;
using DG.Tweening.Core;
using Economic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Economic.DropItems
{
    public class EItemDrop : MonoBehaviour, ICollectible
    {
        private const float FlySpeed = 10f;
        
        [SerializeField] private GameObject vfxClaim;
        [SerializeField] private GameObject visual;
        
        public float CollectDuration { get; set; }
        
        public WealthType kind;
        public int Quantity { get; set; }
        [NonSerialized] public Vector3 vfxPositionOffset;
        
        public void Drop(Vector2 position)
        {
            visual.gameObject.SetActive(true);
            var targetPos = position + Random.insideUnitCircle.normalized * 0.6f;
            transform.DOJump(targetPos, 0.2f, 1, 0.5f).SetTarget(this);
        }

        public bool CanCollectByMouse { get; set; }
        public void Collect(Transform target, float delay)
        {
            CanCollectByMouse = false;
            if (Quantity > 0)
            {
                switch (kind)
                {
                    case WealthType.Vestige:
                        WealthManager.Instance.AddDark(Quantity);
                        break;
                    case WealthType.Echoes:
                        WealthManager.Instance.AddLevelPoint(Quantity);
                        break;
                    case WealthType.Sigils:
                        WealthManager.Instance.AddBossPoint(Quantity);
                        break;
                }
            }
            
            Collect(target, delay, () =>
            {
                EItemDropPool.Instance.Release(this);
            });
        }

        public void Collect(Transform target, float delay, Action onCollected)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetDelay(delay)
                .Append(transform.DOMove(target.position, CollectDuration).SetEase(Ease.InBack))
                .AppendCallback(() =>
                {
                    visual.gameObject.SetActive(false);
                    vfxClaim.transform.position = target.position + vfxPositionOffset;
                    vfxClaim.SetActive(CanCollectByMouse);
                });
            if (CanCollectByMouse)
                seq.AppendInterval(1f);

            seq.OnComplete(() =>
            {
                vfxClaim.SetActive(false);
                onCollected?.Invoke();
            });
        }
    }
}