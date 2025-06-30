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

        public void PlayAttack()
        {
            spritesAnim.PlayAttack();
        }

        public void PlayDie()
        {
            spritesAnim.PlaySpecialAttack();   
        }
    }
}