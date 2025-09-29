using System;
using System.Linq;
using UnityEngine;

namespace Dark.Scripts.Common.ParticleSystemUtil
{
    public class ParticleSystemGroup : MonoBehaviour
    {
        private ParticleSystem[] childPS;
        private ParticleSystem.MainModule[] childMM;

        private void Awake()
        {
            childPS = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            if (childPS is { Length: > 0 })
                childMM = new ParticleSystem.MainModule[childPS.Length];
        }

        public void Play()
        {
            if (childPS is { Length: > 0 })
                childPS[0].Play();
        }

        public void ScaleDuration(float scale)
        {
            if (childPS is { Length: > 0 })
            {
                for (var i = 0; i < childPS.Length; i++)
                {
                    childMM[i] = childPS[i].main;
                    childMM[i].duration *= scale;
                }
            }
        }

        public void ScaleStartLifetime(float scale)
        {
            if (childPS is { Length: > 0 })
            {
                for (var i = 0; i < childPS.Length; i++)
                {
                    childMM[i] = childPS[i].main;
                    childMM[i].startLifetime = new ParticleSystem.MinMaxCurve(
                        childMM[i].startLifetime.constantMin * scale,
                        childMM[i].startLifetime.constantMax * scale);
                }
            }
        }
    }
}