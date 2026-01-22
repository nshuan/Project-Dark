using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class MoveTowerHelper : SerializedMonoSingleton<MoveTowerHelper>
    {
        [SerializeField] private Dictionary<int, Dictionary<int, SpriteRenderer>> lineConnectTowers;
        [SerializeField] private Color startColor;

        public SpriteRenderer GetTowerLine(int from, int to)
        {
            if (lineConnectTowers == null) return null;
            SpriteRenderer line = null;
            
            if (lineConnectTowers.TryGetValue(from, out var linesFrom))
            {
                if (linesFrom.TryGetValue(to, out var targetLine)) line = targetLine;
            }

            if (!line)
            {
                if (lineConnectTowers.TryGetValue(to, out var linesTo))
                {
                    if (linesTo.TryGetValue(from, out var targetLine)) line = targetLine;
                }
            }

            if (!line) return null;
            
            line.transform.localScale = new Vector3(line.transform.localScale.x, 0f, line.transform.localScale.z);
            line.color = startColor;
            line.gameObject.SetActive(true);
            return line;
        }

        public SpriteRenderer GetTowerLine(TowerEntity from, TowerEntity to)
        {
            if (lineConnectTowers == null) return null;
            SpriteRenderer line = null;
            
            if (lineConnectTowers.TryGetValue(from.Id, out var linesFrom))
            {
                if (linesFrom.TryGetValue(to.Id, out var targetLine)) line = targetLine;
            }

            if (!line)
            {
                if (lineConnectTowers.TryGetValue(to.Id, out var linesTo))
                {
                    if (linesTo.TryGetValue(from.Id, out var targetLine)) line = targetLine;
                }
            }

            if (!line) return null;
            
            // Hướng mũi tên
            var sign = 1f;
            if (from.Id > to.Id) sign = -1;
            
            line.transform.localScale = new Vector3(sign * Mathf.Abs(line.transform.localScale.x), 0f, line.transform.localScale.z);
            line.gameObject.SetActive(true);
            return line;
        }
    }
}