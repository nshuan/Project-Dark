using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class PlayerAnimController : MonoBehaviour
    {
        [SerializeField] private Player8DirectionsAnimation spritesAnim;
        [SerializeField] private Transform chargeEffectLower;
        [SerializeField] private Transform chargeEffectUpper;

        private void Awake()
        {
            spritesAnim.SetChargeFx(chargeEffectLower, chargeEffectUpper);
        }

        public void PlayIdle()
        {
            spritesAnim.PlayIdle();    
        }

        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        // (before attack, full duration)
        public (float, float) PlayAttack()
        {
            return spritesAnim.PlayAttack();
        }

        public float PlayCharge()
        {
            return spritesAnim.PlayCharge();
        }

        public void EndChargeAndShoot()
        {
            spritesAnim.EndChargeAndShoot();
        }

        public void PlayDie()
        {
            spritesAnim.PlaySpecialAttack();   
        }

        public void SetDirection(Vector2 direction)
        {
            spritesAnim.UpdateRotation(direction);
        }
    }
}