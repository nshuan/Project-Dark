using Core;
using UnityEngine;

namespace InGame
{
    public class MoveTowerHelper : MonoSingleton<MoveTowerHelper>
    {
        [SerializeField] private SpriteRenderer[] lineConnectTowers;
        [SerializeField] private Color startColor;

        public SpriteRenderer GetTowerLine(int from, int to)
        {
            var index = 3 - from - to;
            var line = lineConnectTowers[index];
            line.transform.localScale = new Vector3(line.transform.localScale.x, 0f, line.transform.localScale.z);
            line.color = startColor;
            line.gameObject.SetActive(true);
            return line;
        }

        public SpriteRenderer GetTowerLine(TowerEntity from, TowerEntity to)
        {
            var index = 3 - from.Id - to.Id;
            if (index < 0 || index >= lineConnectTowers.Length) return null;
            
            // Hướng mũi tên
            var sign = 1f;
            if (from.Id == 1 && to.Id == 0) sign = -1;
            else if (from.Id == 2 && to.Id == 1) sign = -1;
            else if (from.Id == 0 && to.Id == 2) sign = -1;
            
            var line = lineConnectTowers[index];
            line.transform.localScale = new Vector3(sign * Mathf.Abs(line.transform.localScale.x), 0f, line.transform.localScale.z);
            line.gameObject.SetActive(true);
            return line;
        }
    }
}