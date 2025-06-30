using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyAnimController : MonoBehaviour
    {
        [SerializeField] private MonoEnemyHitEffect enemyHitEffect;
        [SerializeField] private EnemySpritesAnimation spritesAnim;

        public void SetDefaultRun()
        {
                
        }
        
        public void PlayIdle()
        {
            spritesAnim.PlayIdle();    
        }

        public void PlayRun()
        {
            spritesAnim.PlayRun();    
        }
        
        public void PlayHit()
        {
            enemyHitEffect.OnHit();
        }

        public void PlayAttack()
        {
            spritesAnim.PlayAttack();
        }

        public void PlayDie()
        {
            spritesAnim.PlayDie();   
        }
    }
}