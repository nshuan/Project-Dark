using System;
using UnityEngine;

namespace InGame
{
    public class ProjectileDead : MonoBehaviour
    {
        [SerializeField] private LayerMask gateLayer;
        
        private RaycastHit2D[] hits = new RaycastHit2D[1];
        
        private void OnEnable()
        {
            CheckSwallowedByGate();
        }

        private void CheckSwallowedByGate()
        {
            var gateHit = Physics2D.CircleCastNonAlloc(transform.position, 0.1f, Vector2.zero, hits, 0.1f, gateLayer);
            if (gateHit > 0)
            {
                Swallowed();
            }
        }

        protected virtual void Swallowed()
        {
            gameObject.SetActive(false);
        }
    }
}