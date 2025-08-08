using UnityEngine;

namespace Dark.Scripts.Audio
{
    public class AudioComponent : MonoBehaviour
    {
        public int sourceIndex;
        public bool playOnEnable = false;
        public float delay;

        private AudioSource source;

        void OnEnable()
        {
            if (playOnEnable)
                Play();
        }
        
        /// <summary>
        /// Play audio immediately or after delay
        /// </summary>
        public void Play()
        {
            AudioManager.Instance.PlaySFX(sourceIndex, delay:delay);
        }
    }

}