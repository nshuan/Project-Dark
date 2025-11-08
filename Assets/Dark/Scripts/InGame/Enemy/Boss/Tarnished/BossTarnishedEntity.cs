using System.Collections;
using InGame.CameraController;
using UnityEngine;

namespace InGame.Boss
{
    public class BossTarnishedEntity : EnemyEntity
    {
        protected override IEnumerator IEDie(float delayRelease)
        {
            // Đợi chạy xong anim hit rồi mới chạy anim die
            yield return new WaitForSeconds(0.3f);
            animController.PlayIdle();
            InGameCameraController.Instance.ZoomToPosition(transform.position);  
            yield return new WaitForSeconds(0.8f);
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(animController.PlayDie());
            CombatActions.OnCollectResource?.Invoke(this);
            Time.timeScale = 1;
            OnDead?.Invoke();
            OnDead = null;
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
        }

        protected override void CollectResource()
        {
            
        }
    }
}