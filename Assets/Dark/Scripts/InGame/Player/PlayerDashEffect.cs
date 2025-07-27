using UnityEngine;

namespace InGame
{
    public class PlayerDashEffect : MonoBehaviour
    {
        [SerializeField] private GameObject vfxDash;
        
        public void PLayStart(Vector2 direction)
        {
            vfxDash.SetActive(true);
        }

        public void PLayEnd()
        {
            vfxDash.SetActive(false);
        }
    }
}