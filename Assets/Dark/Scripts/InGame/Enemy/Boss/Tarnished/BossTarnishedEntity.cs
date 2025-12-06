using System.Collections;
using InGame.CameraController;
using InGame.UI;
using UnityEngine;

namespace InGame.Boss
{
    public class BossTarnishedEntity : EnemyEntity
    {
        protected override IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            // Làm đen hết màn hình, tắt UI
            BackgroundInGame.Instance.SetActiveBlackBg(true);
            CanvasInGame.Instance.HideUI();
            
            CombatActions.OnBossKilled?.Invoke(config, transform.position);
            CombatActions.OnDropResource?.Invoke(this);
            OnDead?.Invoke(reason);
            OnDead = null;
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
        }

        protected override void DropResource()
        {
            
        }
    }
}