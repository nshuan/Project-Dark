using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyAnimController : MonoBehaviour
    {
        [SerializeField] private MonoEnemyHitEffect enemyHitEffect;
        [SerializeField] private EnemySpritesAnimation spritesAnim;

        [Button]
        public void SetDefaultRun(bool defaultRun)
        {
            spritesAnim.isDefaultRun = defaultRun;
        }

        [Button]
        public float PlayCustomAnim(EnemySpritesAnimationInfo anim)
        {
            return spritesAnim.PlayCustomAnim(anim);
        }

        [Button]
        public float PlaySpawn()
        {
            return spritesAnim.PlaySpawn();
        }
        
        [Button]
        public void PlayIdle()
        {
            spritesAnim.PlayIdle();    
        }

        [Button]
        public void PlayRun()
        {
            spritesAnim.PlayRun();    
        }
        
        [Button]
        public void PlayHit()
        {
            enemyHitEffect.OnHit();
        }

        [Button]
        public float PlayAttack()
        {
            return spritesAnim.PlayAttack();
        }

        [Button]
        public float PlayDie()
        {
            return spritesAnim.PlayDie();   
        }

        public void Pause()
        {
            spritesAnim.Pause();
        }

        public void Resume()
        {
            spritesAnim.Resume();
        }
    }
}