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
            var line = lineConnectTowers[index];
            var direction = to.GetBaseCenter() - from.GetBaseCenter();
            line.transform.localScale = new Vector3(direction.magnitude, 0f, line.transform.localScale.z);
            line.transform.position = (to.GetBaseCenter() + from.GetBaseCenter()) / 2;
            line.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            line.color = startColor;
            line.gameObject.SetActive(true);
            return line;
        }
    }
}