using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.Utils;
using DG.Tweening;
using Economic.InGame.DropItems;
using InGame.EnemyEffect;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Boss
{
    public class BossKilledAnim : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private Dictionary<int, GameObject> bossAnimDict;
        [SerializeField] private Vector2 bossTargetPosition;
        [SerializeField] private float bossTargetScale = 1.8f;

        [Header("Fake Item drop")] 
        [SerializeField] private EItemDrop fakeVestigePrefab;
        [SerializeField] private Transform fakeSigils;
        [SerializeField] private ParticleSystem vfxSpawnFakeSigils;

        private List<EItemDrop> fakeVestige = new List<EItemDrop>();
        
        private void Awake()
        {
            CombatActions.OnBossKilled += OnBossKilled;
        }

        private void OnDestroy()
        {
            CombatActions.OnBossKilled -= OnBossKilled;
        }

        private void OnBossKilled(EnemyBehaviour bossConfig, Vector2 position)
        {
            BackgroundInGame.Instance.SetActiveBlackAll(true);
            if (bossAnimDict == null || !bossAnimDict.TryGetValue(bossConfig.enemyId, out var deadBoss)) return;
            StartCoroutine(IEBossAnim(deadBoss, position));
        }

        private IEnumerator IEBossAnim(GameObject deadBoss, Vector2 position)
        {
            var animController = deadBoss.GetComponentInChildren<EnemyAnimController>(includeInactive:true);
            deadBoss.transform.position = position;
            animController.PlayIdle();
            deadBoss.gameObject.SetActive(true);

            yield return DOTween.Sequence(deadBoss).SetUpdate(true)
                .Append(deadBoss.transform.DOScale(bossTargetScale, 0.5f).SetEase(Ease.OutQuad))
                .Join(deadBoss.transform.DOMove(bossTargetPosition, 0.5f).SetEase(Ease.OutQuad))
                .WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
            
            Time.timeScale = 0.4f;
            animController.PlayDie();
            yield return new WaitForSeconds(0.5f);
            DropFakeVestige(deadBoss);
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1f;
            DropFakeSigils(deadBoss);
        }

        private void DropFakeVestige(GameObject deadBoss)
        {
            var dropPosition = deadBoss.transform.position + new Vector3(-0.2f, 0.9f, 0f);
            for (int i = 0; i < 20; i++)
            {
                var item = Instantiate(fakeVestigePrefab, transform);
                item.Quantity = 0;
                item.transform.position = dropPosition;
                item.transform.localScale = bossTargetScale * Vector3.one;
                fakeVestige.Add(item);
                item.gameObject.SetActive(true);
                item.Drop(dropPosition, Vector2.right, 80f);
            }
            
            for (int i = 0; i < 20; i++)
            {
                var item = Instantiate(fakeVestigePrefab, transform);
                item.Quantity = 0;
                item.transform.position = dropPosition;
                item.transform.localScale = bossTargetScale * Vector3.one;
                fakeVestige.Add(item);
                item.gameObject.SetActive(true);
                item.Drop(dropPosition, Vector2.left, 80f);
            }
        }

        private void DropFakeSigils(GameObject deadBoss)
        {
            var dropPosition = deadBoss.transform.position + new Vector3(-0.2f, 1.2f, 0f);
            vfxSpawnFakeSigils.transform.position = dropPosition;
            fakeSigils.position = dropPosition;
            fakeSigils.localScale = Vector3.zero;
            fakeSigils.gameObject.SetActive(true);
            vfxSpawnFakeSigils.Play();
            fakeSigils.DOScale(1f, 0.1f).SetEase(Ease.OutQuad).SetDelay(1f);
        }
    }
}