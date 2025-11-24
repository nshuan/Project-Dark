using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    public class VfxExplosionPlane : MonoBehaviour
    {
        public static float Angle { get;private set; }
        
        [Range(-90f, 0f)] [SerializeField] private float angle = -40f;

        private void Awake()
        {
            Angle = transform.eulerAngles.x;
        }

        [Button]
        private void OnValidate()
        {
            transform.rotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}