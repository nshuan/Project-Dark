using System;
using UnityEngine;

namespace InGame
{
    public class LightningLineRenderer : MonoBehaviour
    {
        [SerializeField] private Transform vfxParent;
        [SerializeField] private LineRenderer[] lineRenderers;

        private Vector3 hidePos = new Vector3(99999, 99999, 99999);
        private Vector3 collapsePos = new Vector3();
        private int targetCount;
        private Transform[] targets;
        private Vector3[] targetPositions;
        private Transform[] lightningBalls;
        private int startLineIndex;
        private int endLineIndex;
        
        public void Initialize(int maxAnchor)
        {
            foreach (var line in lineRenderers)
            {
                line.positionCount = maxAnchor;
            }
            
            targets = new Transform[maxAnchor];
            targetPositions = new Vector3[maxAnchor];
            lightningBalls = new Transform[maxAnchor];
            for (var i = 0; i < maxAnchor; i++)
            {
                targetPositions[i] = new Vector3();
            }
        }

        private void Update()
        {
            if (targets == null) return;
            
            // Fill in positions from targets
            // Collapse the rest to the last valid point
            for (var i = startLineIndex; i <= endLineIndex; i++)
            {
                targetPositions[i].x = targets[i].position.x;
                targetPositions[i].y = targets[i].position.y;
                targetPositions[i].z = targets[i].position.z;
                lightningBalls[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < startLineIndex; i++)
            {
                targetPositions[i].x = targetPositions[startLineIndex].x;
                targetPositions[i].y = targetPositions[startLineIndex].y;
                targetPositions[i].z = targetPositions[startLineIndex].z;
            }
            if (targetCount > 0)
            {
                collapsePos.x = targetPositions[endLineIndex].x;
                collapsePos.y = targetPositions[endLineIndex].y;
                collapsePos.z = targetPositions[endLineIndex].z;
            }
            for (int i = endLineIndex + 1; i < targetPositions.Length; i++)
            {
                targetPositions[i].x = collapsePos.x;
                targetPositions[i].y = collapsePos.y;
                targetPositions[i].z = collapsePos.z;
            }

            // Apply all at once
            foreach (var line in lineRenderers)
            {
                line.SetPositions(targetPositions);
            }
            
            for (var i = 0; i < lightningBalls.Length; i++)
            {
                if (!lightningBalls[i]) break;
                
                if (i < startLineIndex || i > endLineIndex)
                {
                    lightningBalls[i].gameObject.SetActive(false);
                    continue;
                }
                
                lightningBalls[i].position = targetPositions[i];
                lightningBalls[i].gameObject.SetActive(true);
            }
        }

        public void ResetLine(int maxAnchor, Transform[] target, int targetValidCount)
        {
            targetCount = targetValidCount;
            startLineIndex = 0;
            endLineIndex = 0;

            if (targetCount > 0)
            {
                for (var i = 0; i < targetCount; i++)
                {
                    targets[i] = target[i];
                    targetPositions[i].x = target[i].position.x;
                    targetPositions[i].y = target[i].position.y;
                    targetPositions[i].z = target[i].position.z;
                    lightningBalls[i] ??= LightningBallPool.Instance.Get(transform.parent, false).transform;
                }
            }
            
            foreach (var line in lineRenderers)
            {
                for (var i = 0; i < maxAnchor; i++)
                {
                    line.SetPosition(i, hidePos);
                }
            }
        }
        
        public void ActiveAnchor(int index, bool active)
        {
            if (index >= targetCount) return;
            if (index <= 0) return;

            if (active && index > endLineIndex)
            {
                endLineIndex = index;
            }

            if (!active && index < endLineIndex)
            {
                startLineIndex = index + 1;
            }
        }
        
        private void OnValidate()
        {
            lineRenderers = vfxParent.GetComponentsInChildren<LineRenderer>();
        }
    }
}