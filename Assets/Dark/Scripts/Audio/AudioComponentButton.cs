using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Audio
{
    public class AudioComponentButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public int sourceIndex;
        public float delay;
        public AudioButtonType audioType;
        
        /// <summary>
        /// Play audio immediately or after delay
        /// </summary>
        [Button]
        public void Play()
        {
            AudioManager.Instance.PlaySFX(sourceIndex, delay:delay);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (audioType == AudioButtonType.OnCLick) Play();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (audioType == AudioButtonType.OnHover) Play();
        }
        
        public enum AudioButtonType
        {
            OnCLick,
            OnHover
        }
    }
}