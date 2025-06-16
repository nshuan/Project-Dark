using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.Player {
    public class MapReposition : MonoBehaviour
    {
        [SerializeField] private Player _player;
        private void Awake() {
            _player = FindObjectOfType<Player>();
        }
        
        private void OnTriggerExit2D(Collider2D other) {
            
            if (!other.CompareTag("Area") || _player == null)
                return;
            
            Vector3 playerPos = _player.transform.position;
            Vector3 thisMapPiecePos = transform.position;

            float dx = Mathf.Abs(playerPos.x - thisMapPiecePos.x);
            float dy = Mathf.Abs(playerPos.y - thisMapPiecePos.y);

            Vector2 playerDir = _player.GetComponent<Player>().ActualVelocity;
        
            float dirX = playerDir.x < 0 ? -1 : 1;
            float dirY = playerDir.y < 0 ? -1 : 1;

            if (dx > dy)
                transform.Translate(Vector3.right * dirX * 64);
            else if (dx < dy)
                transform.Translate(Vector3.up * dirY * 64);
        
        }

    }
}
