using UnityEngine;

namespace InGame
{
    public class PlayerAnimController : MonoBehaviour
    {
        [SerializeField] private PlayerSpineAnimation spritesAnim;
        
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