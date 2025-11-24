using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.Utils;
using DG.Tweening;
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
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1f;
        }
    }
}