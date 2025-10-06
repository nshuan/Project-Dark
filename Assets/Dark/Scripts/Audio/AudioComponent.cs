using Sirenix.OdinInspector;
using UnityEngine;

namespace Dark.Scripts.Audio
{
    public class AudioComponent : MonoBehaviour
    {
        public int sourceIndex;
        public bool playOnEnable = false;
        public float delay;
        
        void OnEnable()
        {
            if (playOnEnable)
                Play();
        }
        
        /// <summary>
        /// Play audio immediately or after delay
        /// </summary>
        [Button]
        public void Play()
        {
            AudioManager.Instance.PlaySFX(sourceIndex, delay:delay);
        }
    }

}