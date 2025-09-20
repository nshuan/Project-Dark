using System;
using UnityEngine;

namespace InGame
{
    public class MoveCollectResourceCursor : MonoBehaviour
    {
        public Transform Target { get; set; }
        public bool Enabled { get; set; }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!Enabled) return;
            if (other.CompareTag("Collectible"))
            {
                Collect(other.gameObject);
            }
        }

        private void Collect(GameObject obj)
        {
            if (obj.TryGetComponent<ICollectible>(out var collectible))
            {
                if (collectible.CanCollectByMouse)
                    collectible.Collect(Target, 0f);
            }
        }
    }
}