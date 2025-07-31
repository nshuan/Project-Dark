using UnityEngine;

namespace InGame
{
    public class PlayerDashEffect : MonoBehaviour
    {
        [SerializeField] private GameObject vfxDash;
        
        public void PLayStart(Vector2 direction)
        {
            // Calculate angle in degrees
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation around Z axis
            vfxDash.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            
            vfxDash.SetActive(true);
        }

        public void PLayEnd()
        {
            vfxDash.SetActive(false);
        }
    }
}