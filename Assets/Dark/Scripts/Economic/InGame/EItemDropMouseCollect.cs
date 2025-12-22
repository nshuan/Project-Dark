using System;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropMouseCollect : MonoBehaviour
    {
        [SerializeField] private float radius;

        private Camera mainCam;
        private RaycastHit2D[] hits = new RaycastHit2D[50];

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}