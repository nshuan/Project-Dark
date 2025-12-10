using System;
using Dark.Scripts.Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.AudioV2
{
    public class AudioPlayOnClickV2 : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string cueKey;
        [SerializeField] private AudioChannel channel = AudioChannel.Ui;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (channel)
            {
                case AudioChannel.InGame:
                    AudioManagerV2.Instance.PlayInGame(cueKey);
                    break;
                case AudioChannel.OutGame:
                    AudioManagerV2.Instance.PlayOutGame(cueKey);
                    break;
                case AudioChannel.Ui:
                    AudioManagerV2.Instance.PlayUi(cueKey);
                    break;
            }
        }
    }
}