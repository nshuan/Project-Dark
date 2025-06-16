using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeatBullets.SpaceShooter.Scripts.Managers {
    
    public class AudioManager : MonoBehaviour {

        // Assign in editor
        public AudioClip PlayerShoot;
        public AudioClip PlayerHurt;
        public AudioClip EnemyShoot;
        public AudioClip EnemyHurt;
        
        [SerializeField] private AudioClip _music1;
        [SerializeField] private AudioClip _music2;
        [SerializeField] private AudioClip _music3;
        [SerializeField] private List<AudioClip> _musicClips = new List<AudioClip>();
        
        private AudioSource _audioSource;
        
        
        public static AudioManager Instance;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }

            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            
            _musicClips.AddRange(new[] { _music1, _music2, _music3 });
            _audioSource.clip = _musicClips[Random.Range(0, _musicClips.Count)];
            
            _audioSource.Play();
        }

        [SerializeField] private bool _isPlayingSoundEffects = true;
        public void PlayAudioEffect(AudioSource src, AudioClip clip) {
            if (!_isPlayingSoundEffects)
                return;
            
            src.PlayOneShot(clip);
        }
        
        [HideInInspector] [SerializeField] private bool _isPlayingMusic = true;
        public bool IsPlayingMusic {
            get => _isPlayingMusic; 
            set {
                _isPlayingMusic = value;
                PlayOrStopMusic();
            }  
        }
        
        
        private void PlayOrStopMusic() {
            if (_isPlayingMusic) {
                _audioSource.Play();
            }
            else {
                _audioSource.Stop();
            }
            
        }
        
        

        












    }
}
