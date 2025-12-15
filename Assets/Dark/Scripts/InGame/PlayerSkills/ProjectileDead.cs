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
            if (GateManager.Instance.ListGateInLevel == null) return;
            foreach (var gate in GateManager.Instance.ListGateInLevel)
            {
                if (gate.IsActive && gate.IsInGate(transform.position))
                {
                    Swallowed();
                    return;
                }
            }
        }

        protected virtual void Swallowed()
        {
            gameObject.SetActive(false);
        }
    }
}