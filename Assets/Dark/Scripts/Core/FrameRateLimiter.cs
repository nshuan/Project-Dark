using UnityEngine;

namespace Core
{
    public class FrameRateLimiter : MonoBehaviour
    {
        public int targetFps = 120;
        
        void Awake()
        {
            // Prevent duplicates
            if (FindObjectsOfType<FrameRateLimiter>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFps;
        }
    }
}