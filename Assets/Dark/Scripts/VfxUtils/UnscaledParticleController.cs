using System;
using UnityEngine;

namespace Dark.Scripts.VfxUtils
{
    public class UnscaledParticleController : MonoBehaviour
    {
        private ParticleSystem ps;

        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            ps.Play(); // Start manually
        }

        void Update()
        {
            // Simulate with unscaled deltaTime
            ps.Simulate(Time.unscaledDeltaTime, withChildren: true, restart: false);
        }
    }

}