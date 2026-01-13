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
        
        public float GetCustomAnimDuration(EnemySpritesAnimationInfo anim) => spritesAnim.GetCustomAnimDuration(anim);

        [Button]
        public float PlaySpawn()
        {
            return spritesAnim.PlaySpawn();
        }
        
        public float GetSpawnDuration() => spritesAnim.GetSpawnDuration();
        
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

        public float GetAttackDelayTrigger() => spritesAnim.GetAttackDelayTrigger();
        public float GetAttackDuration() => spritesAnim.GetAttackDuration();

        [Button]
        public float PlayDie()
        {
            return spritesAnim.PlayDie();   
        }

        public float GetDieDuration() => spritesAnim.GetDieDuration();

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