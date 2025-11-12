using System;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public class LightningLineRendererV2 : MonoBehaviour
    {
        [SerializeField] private Transform vfxParent;
        [SerializeField] private LineRenderer[] lineRenderers;

        private LightningBall[] targets;
        
        public void Initialize()
        {
            foreach (var line in lineRenderers)
            {
                line.positionCount = 0;
            }
        }

        private void Update()
        {
            if (targets == null) return;

            var activeAnchor = targets.Count((t) => t && t.gameObject.activeInHierarchy);
            foreach (var line in lineRenderers)
            {
                line.positionCount = activeAnchor;
            }

            var linePositionIndex = 0;
            for (var i = 0; i < targets.Length; i++)
            {
                if (!targets[i] || !targets[i].gameObject.activeInHierarchy) continue;
                foreach (var line in lineRenderers)
                {
                    line.SetPosition(linePositionIndex, targets[i].transform.position);
                }

                linePositionIndex += 1;
            }
        }

        public void ResetLine(LightningBall[] target)
        {
            if (targets != null)
            {
                foreach (var t in targets)
                {
                    if (t) LightningBallPool.Instance.Release(t);
                }
            }
            
            targets = target;
            if (targets != null)
            {
                foreach (var t in targets)
                {
                    if (t) t.gameObject.SetActive(false);
                }
            }
            
            foreach (var line in lineRenderers)
            {
                line.positionCount = 0;
                line.SetPositions(Array.Empty<Vector3>());
            }
        }
        
        public void ActiveAnchor(int index, bool active)
        {
            if (targets == null) return;
            if (index >= targets.Length) return;
            if (index <= 0) return;

            targets[index].gameObject.SetActive(active);
        }
        
        private void OnValidate()
        {
            lineRenderers = vfxParent.GetComponentsInChildren<LineRenderer>();
        }
    }
}