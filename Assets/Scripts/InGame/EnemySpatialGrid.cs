using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class EnemySpatialGrid
    {
        private Dictionary<Vector2Int, List<EnemyBoidAgent>> cells = new Dictionary<Vector2Int, List<EnemyBoidAgent>>();
        private float cellSize;
        
        public EnemySpatialGrid(float width, float height, float cellSize)
        {
            this.cellSize = cellSize;
        }

        Vector2Int GetCell(Vector3 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.z / cellSize));
        }
        
        public void Register(EnemyBoidAgent agent)
        {
            var cell = GetCell(agent.transform.position);
            if (!cells.ContainsKey(cell)) cells[cell] = new List<EnemyBoidAgent>();
            if (!cells[cell].Contains(agent))
                cells[cell].Add(agent);
        }
        
        public void Clear() => cells.Clear();

        public List<EnemyBoidAgent> GetNearby(EnemyBoidAgent agent, float radius)
        {
            var result = new List<EnemyBoidAgent>();
            var baseCell = GetCell(agent.transform.position);
            var r = Mathf.CeilToInt(radius / cellSize);

            for (var x = -r; x <= r; x++)
            {
                for (var y = -r; y <= r; y++)
                {
                    var checkCell = baseCell + new Vector2Int(x, y);
                    if (cells.TryGetValue(checkCell, out var agents))
                    {
                        foreach (var a in agents)
                        {
                            if (a != agent && a.IsActive && Vector3.Distance(a.transform.position, agent.transform.position) < radius)
                                result.Add(a);
                        }
                    }
                }
            }

            return result;
        }
    }
}