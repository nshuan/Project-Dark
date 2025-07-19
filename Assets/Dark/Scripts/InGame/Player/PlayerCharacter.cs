using System;
using System.Collections;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private DashGhostEffect dashEffect;

        [Space] [SerializeField] private WeaponSupporter weapon;
        public WeaponSupporter Weapon => weapon;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;

        private bool blockRotate;
        
        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        public float PlayShoot(Vector2 target)
        {
            // transform.localScale =
            //     new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            var attackDuration = animController.PlayAttack();
            blockRotate = true;
            StartCoroutine(IEBlockRotate(attackDuration.Item2));
            return attackDuration.Item1;
        }

        private IEnumerator IEBlockRotate(float duration)
        {
            yield return new WaitForSeconds(duration);
            blockRotate = false;
        }

        public void SetDirection(Vector3 target)
        {
            if (blockRotate) return;
            animController.SetDirection(target - transform.position);
        }

        public void PlayDashEffect()
        {
            dashEffect.DoEffect();
        }

        public void StopDashEffect()
        {
            dashEffect.StopEffect();
        }
    }
}