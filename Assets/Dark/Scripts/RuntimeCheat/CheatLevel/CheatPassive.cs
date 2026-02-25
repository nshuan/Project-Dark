using System;
using System.Linq;
using InGame;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat.CheatLevel
{
    public class CheatPassive : MonoBehaviour
    {
        [SerializeField] private Button btnTriggerLightningRandom;
        [SerializeField] private Button btnTriggerExplosionRandom;
        [SerializeField] private Button btnTriggerBurnAll;
        [SerializeField] private Button btnTriggerThunderAll;

        private void Awake()
        {
            btnTriggerBurnAll.onClick.RemoveAllListeners();
            btnTriggerBurnAll.onClick.AddListener(() =>
            {
                foreach (var enemy in EnemyManager.Instance.Enemies)
                {
                    if (enemy.Value.IsDestroyed) continue;
                    PassiveEffectManager.Instance.ForceTriggerEffect(PassiveTriggerType.DameByNormalAttack, PassiveType.Burning, enemy.Value);
                }
            });
            
            btnTriggerThunderAll.onClick.RemoveAllListeners();
            btnTriggerThunderAll.onClick.AddListener(() =>
            {
                foreach (var enemy in EnemyManager.Instance.Enemies)
                {
                    if (enemy.Value.IsDestroyed) continue;
                    PassiveEffectManager.Instance.ForceTriggerEffect(PassiveTriggerType.DameByNormalAttack, PassiveType.Thunder, enemy.Value);
                }
            });
            
            btnTriggerExplosionRandom.onClick.RemoveAllListeners();
            btnTriggerExplosionRandom.onClick.AddListener(() =>
            {
                var alive = EnemyManager.Instance.Enemies.Where((e) => e.Value.IsDestroyed == false).Select((e) => e.Value).ToArray();
                if (alive.Length == 0) return;
                var randomTarget = alive[RandomUtil.Range(0, alive.Length)];
                PassiveEffectManager.Instance.ForceTriggerEffect(PassiveTriggerType.DameByNormalAttack, PassiveType.Explosion, randomTarget);
            });
            
            btnTriggerLightningRandom.onClick.RemoveAllListeners();
            btnTriggerLightningRandom.onClick.AddListener(() =>
            {
                var alive = EnemyManager.Instance.Enemies.Where((e) => e.Value.IsDestroyed == false).Select((e) => e.Value).ToArray();
                if (alive.Length == 0) return;
                var randomTarget = alive[RandomUtil.Range(0, alive.Length)];
                PassiveEffectManager.Instance.ForceTriggerEffect(PassiveTriggerType.DameByNormalAttack, PassiveType.Lightning, randomTarget);
            });
        }
    }
}