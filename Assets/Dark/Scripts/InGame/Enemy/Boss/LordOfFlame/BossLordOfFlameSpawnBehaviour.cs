using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame.Boss
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Boss Lord of Flame Spawn", fileName = "BossLordOfFlameSpawn")]
    public class BossLordOfFlameSpawnBehaviour : EnemySpawnBehaviour
    {
        [SerializeField] private float delayShake1AfterSpawn;
        [SerializeField] private float delayShake2AfterSpawn;
        
        private CameraShake cameraShake1;
        private CameraShake cameraShake2;
        
        public override void Init(EnemyEntity enemy)
        {
            cameraShake1 = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 0.3f, Magnitude = 0.06f };
            cameraShake2 = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 0.6f, Magnitude = 0.1f };
        }

        public override Tween DoSpawn(EnemyEntity enemy)
        {
            if (enemy is BossLordOfFlameEntity boss)
            {
                var spawnDuration = enemy.animController.GetSpawnDuration();
                var comboAttackDuration = enemy.animController.GetCustomAnimDuration(boss.comboAttackAnim);
                return DOTween.Sequence(enemy)
                    .AppendCallback(() => enemy.animController.PlaySpawn())
                    .AppendInterval(spawnDuration)
                    .AppendInterval(0.1f)
                    .AppendCallback(() =>
                    {
                        DOTween.Sequence()
                            .AppendInterval(delayShake1AfterSpawn)
                            .AppendCallback(() => VisualEffectHelper.Instance.PlayEffect(cameraShake1))
                            .AppendInterval(delayShake2AfterSpawn)
                            .AppendCallback(() => VisualEffectHelper.Instance.PlayEffect(cameraShake2))
                            .Play();
                        
                        enemy.animController.PlayCustomAnim(boss.comboAttackAnim);
                    })
                    .AppendInterval(comboAttackDuration)
                    .AppendInterval(0.8f);
            }
            
            return DOTween.Sequence(enemy)
                .AppendInterval(enemy.animController.PlaySpawn());
        }
    }
}