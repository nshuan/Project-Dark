using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class MoveTowerHelper : SerializedMonoSingleton<MoveTowerHelper>
    {
        [SerializeField] private Transform lineConnectTowers;
        [SerializeField] private SpriteRenderer lineVisual;
        [SerializeField] private Color startColor;

        private Vector2 baseVisualSize;

        private void Start()
        {
            baseVisualSize = lineVisual.size;
        }

        public Transform GetTowerLine(int from, int to)
        {
            if (lineConnectTowers == null) return null;
  
            lineConnectTowers.localScale = new Vector3(lineConnectTowers.localScale.x, 0f, lineConnectTowers.localScale.z);
            lineVisual.color = startColor;
            lineConnectTowers.gameObject.SetActive(true);
            return lineConnectTowers;
        }

        public Transform GetTowerLine(TowerEntity from, TowerEntity to, float size)
        {
            var direction = (to.GetBaseCenter() - from.GetBaseCenter());
            var scaledDir = new Vector3(
                direction.x / lineConnectTowers.parent.lossyScale.x,
                direction.y / lineConnectTowers.parent.lossyScale.y,
                direction.z);
            var distance = scaledDir.magnitude;
            scaledDir.Normalize();
            lineConnectTowers.position = from.GetBaseCenter();
            lineConnectTowers.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(scaledDir.y, scaledDir.x) * Mathf.Rad2Deg);
            lineConnectTowers.gameObject.SetActive(true);
            lineVisual.transform.localScale = new Vector3(1f, size, 1f);
            lineVisual.size = new Vector2(baseVisualSize.x * size, baseVisualSize.y * distance / size);
            return lineConnectTowers;
        }
    }
}