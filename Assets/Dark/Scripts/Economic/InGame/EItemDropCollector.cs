using System;
using DG.Tweening;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropCollector : MonoBehaviour, IDamageable
    {
        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        private int totalItemToCollect;
        private ECollectorData collectedData;

        public void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            CombatActions.OnResourceCollectorDamaged?.Invoke(this);
        }

        public void RegisterItem(EItemDrop item)
        {
            totalItemToCollect += 1;
            switch (item.kind)
            {
                case WealthType.Vestige:
                    collectedData.vestige += item.Quantity;
                    break;
                case WealthType.Echoes:
                    collectedData.echoes += item.Quantity;
                    break;
                case WealthType.Sigils:
                    collectedData.sigils += item.Quantity;
                    break;
            }
        }

        public void CollectItem()
        {
            totalItemToCollect -= 1;
            if (totalItemToCollect <= 0) ClaimAndSpawnNewCollector();
        }

        public void Spawn()
        {
            collectedData = new ECollectorData();
            EItemDropManager.Instance.CollectTarget = this;
            transform.localScale = 0.2f * Vector3.one;
            gameObject.SetActive(true);
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                .Play();
        }

        public void TryHide()
        {
            if (totalItemToCollect > 0) return;
            totalItemToCollect = 0;
            
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(0f, 0.3f).SetEase(Ease.InQuad))
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                })
                .Play();
        }
        
        private void ClaimAndSpawnNewCollector()
        {
            totalItemToCollect = 0;
            collectedData.Claim();

            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(0f, 0.3f).SetEase(Ease.InQuad))
                .AppendCallback(() =>
                {
                    CombatActions.OnSpawnNewItemCollector?.Invoke();
                })
                .Play();
        }

        public bool IsDestroyed { get; set; }
        public Action<int, DamageType> OnHit { get; set; }
    }

    public class ECollectorData
    {
        public int vestige;
        public int sigils;
        public int echoes;

        public void Claim()
        {
            if (vestige > 0)  WealthManager.Instance.AddDark(vestige);
            if (sigils > 0)  WealthManager.Instance.AddBossPoint(sigils);
            if (echoes > 0)  WealthManager.Instance.AddLevelPoint(echoes);

            vestige = 0;
            sigils = 0;
            echoes = 0;
        }
    }
}