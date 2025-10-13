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
    }
}