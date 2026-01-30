using System;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropMouseTrigger : MonoBehaviour
    {
        public CircleCollider2D collider;
        
        private EItemDropMouseAbsorb owner;

        private void Awake()
        {
            collider ??= GetComponent<CircleCollider2D>();
        }

        public void Init(EItemDropMouseAbsorb absorb)
        {
            owner = absorb;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!owner || owner.IsChargeBlockCollect) return;
            owner.TryRegisterItem(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!owner) return;
            owner.TryUnregisterItem(col);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!owner || owner.IsChargeBlockCollect) return;
            owner.TryRegisterItem(other);
        }
    }
}