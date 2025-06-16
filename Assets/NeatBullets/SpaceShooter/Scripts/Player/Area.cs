using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.Player {
    
    // Area is needed for mapReposition
    public class Area : MonoBehaviour {
        private Player _player;
        private void Awake() {
            _player = FindObjectOfType<Player>();
        }

        // Follow player object without rotating with it
        private void LateUpdate() {
            transform.position = _player.transform.position;
        }
    }
}
