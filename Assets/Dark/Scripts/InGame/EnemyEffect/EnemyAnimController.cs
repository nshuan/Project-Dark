using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyAnimController : MonoBehaviour
    {
        [SerializeField] private MonoEnemyHitEffect enemyHitEffect;
        [SerializeField] private EnemySpritesAnimation spritesAnim;

        public void SetDefaultRun(bool defaultRun)
        {
            spritesAnim.isDefaultRun = defaultRun;
        }

        public float PlaySpawn()
        {
            return spritesAnim.PlaySpawn();
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

        public float PlayDie()
        {
            return spritesAnim.PlayDie();   
        }
    }
}