using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public class LightningLineRendererV2 : MonoBehaviour
    {
        [SerializeField] private Transform vfxParent;
        [SerializeField] private LineRenderer[] lineRenderers;

        private List<Transform> targets;
        private int targetCount;
        
        public void Initialize()
        {
            foreach (var line in lineRenderers)
            {
                line.positionCount = 0;
            }
        }

        private void Update()
        {
            if (targetCount == 0) return;
            if (targets == null) return;

            var activeAnchor = targets.Count((t) => t && t.gameObject.activeInHierarchy);
            foreach (var line in lineRenderers)
            {
                line.positionCount = activeAnchor;
            }

            var linePositionIndex = 0;
            for (var i = 0; i < targetCount; i++)
            {
                if (!targets[i] || !targets[i].gameObject.activeInHierarchy) continue;
                foreach (var line in lineRenderers)
                {
                    line.SetPosition(linePositionIndex, targets[i].transform.position);
                }

                linePositionIndex += 1;
            }
        }

        public void ResetLine(Transform[] target)
        {
            targets ??= new List<Transform>();
            for (var i = 0; i < target.Length; i++)
            {
                if (i < targets.Count) targets[i] = target[i];
                else targets.Add(target[i]);
            }
            targetCount = target.Length;
            
            foreach (var line in lineRenderers)
            {
                line.positionCount = 0;
                line.SetPositions(Array.Empty<Vector3>());
            }
        }
        
        private void OnValidate()
        {
            lineRenderers = vfxParent.GetComponentsInChildren<LineRenderer>();
        }
    }
}