using UnityEngine;

namespace Dark.Scripts.AudioV2
{
    public class AudioPlayMusicLoop : MonoBehaviour
    {
        [SerializeField] private string introCueKey;
        [SerializeField] private string loopCueKey;
        [SerializeField] private float fadeDuration = -1f;
        
        private void Start()
        {
            AudioManagerV2.Instance.StopMusic();
            AudioManagerV2.Instance.PlayMusicIntroThenLoop(introCueKey, loopCueKey, fadeDuration);
        }
    }
}