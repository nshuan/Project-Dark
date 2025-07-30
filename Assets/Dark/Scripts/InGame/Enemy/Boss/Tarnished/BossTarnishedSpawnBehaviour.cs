using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame.Boss
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Boss Tarnished Spawn", fileName = "BossTarnishedSpawn")]
    public class BossTarnishedSpawnBehaviour : EnemySpawnBehaviour
    {
        private CameraShake cameraShake;
        
        public override void Init(EnemyEntity enemy)
        {
            enemy.transform.position = new Vector3(0f, 10f, 0f);
            cameraShake = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 1f };
        }

        public override Tween DoSpawn(EnemyEntity enemy)
        {
            return DOTween.Sequence(enemy)
                .Append(enemy.transform.DOMoveY(1.1f, 0.2f).SetEase(Ease.InQuad))
                .AppendCallback(() => VisualEffectHelper.Instance.PlayEffect(cameraShake))
                .AppendInterval(cameraShake.Duration);
        }
    }
}