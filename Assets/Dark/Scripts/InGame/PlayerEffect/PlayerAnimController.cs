using UnityEngine;

namespace InGame
{
    public class PlayerAnimController : MonoBehaviour
    {
        [SerializeField] private PlayerSpritesAnimation spritesAnim;
        
        public void PlayIdle()
        {
            spritesAnim.PlayIdle();    
        }

        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        public float PlayAttack()
        {
            return spritesAnim.PlayAttack();
        }

        public void PlayDie()
        {
            spritesAnim.PlaySpecialAttack();   
        }
    }
}