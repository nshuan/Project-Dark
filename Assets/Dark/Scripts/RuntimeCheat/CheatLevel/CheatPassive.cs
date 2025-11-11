using System;
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
        }
    }
}