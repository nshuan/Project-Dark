using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyHitAnim : MonoEnemyHitEffect
    {
        [SerializeField] private EnemySpritesAnimation animController;
        
        public override void OnHit()
        {
            animController.PlayHit();
        }
    }
}