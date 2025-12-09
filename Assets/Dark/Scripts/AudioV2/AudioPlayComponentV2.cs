using UnityEngine;

namespace Dark.Scripts.AudioV2
{
    public class AudioPlayComponentV2 : MonoBehaviour
    {
        [SerializeField] private string cueKey;
        [SerializeField] private AudioChannel channel = AudioChannel.Ui;

        public void Play()
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