using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class EnemySpatialGrid
    {
        private Dictionary<Vector2Int, List<EnemyBoidAgent>> cells = new Dictionary<Vector2Int, List<EnemyBoidAgent>>();
        private Dictionary<EnemyBoidAgent, Vector2Int> agentCellMap = new Dictionary<EnemyBoidAgent, Vector2Int>();
        private float cellSize;
        
        public EnemySpatialGrid(float width, float height, float cellSize)
        {
            this.cellSize = cellSize;
        }

        void GetCellNonAlloc(Vector3 pos, ref Vector2Int cell)
        {
            cell.x = Mathf.FloorToInt(pos.x / cellSize);
            cell.y = Mathf.FloorToInt(pos.y / cellSize);
        }
        
        public void Register(EnemyBoidAgent agent, ref Vector2Int cell)
        {
            if (agentCellMap.TryGetValue(agent, out cell))
            {
                cells[cell].Remove(agent);
            }
            
            GetCellNonAlloc(agent.transform.position, ref cell);
            agentCellMap[agent] = cell;
            
            if (!cells.ContainsKey(cell)) cells[cell] = new List<EnemyBoidAgent>();
            cells[cell].Add(agent);
        }
        
        public void Clear() => cells.Clear();

        public int GetNearbyNonAlloc(EnemyBoidAgent agent, float radius, ref Vector2Int agentCell, ref List<EnemyBoidAgent> neighbors)
        {
            var count = 0;
            GetCellNonAlloc(agent.transform.position, ref agentCell);
            var r = Mathf.CeilToInt(radius / cellSize);

            for (var x = -r; x <= r; x++)
            {
                for (var y = -r; y <= r; y++)
                {
                    agentCell.x += x;
                    agentCell.y += y;
                    // if (cells.TryGetValue(agentCell, out var agents))
                    // {
                    //     foreach (var a in agents)
                    //     {
                    //         if (a != agent && a.IsActive &&
                    //             Vector3.Distance(a.transform.position, agent.transform.position) < radius)
                    //         {
                    //             if (count >= neighbors.Count)
                    //                 neighbors.Add(a);
                    //             else
                    //                 neighbors[count] = a;
                    //             count += 1;
                    //         }
                    //     }
                    // }
                    if (cells.ContainsKey(agentCell))
                    {
                        neighbors = cells[agentCell];
                        count = neighbors.Count;
                    }
                    
                    agentCell.x -= x;
                    agentCell.y -= y;
                }
            }

            return count;
        }
        
        // 🔧 Visualize partial grid with Gizmos
        public void DrawGizmos()
        {
            int maxAgentsInAnyCell = 1;
            foreach (var kvp in cells)
                if (kvp.Value.Count > maxAgentsInAnyCell)
                    maxAgentsInAnyCell = kvp.Value.Count;

            foreach (var kvp in cells)
            {
                Vector2Int cell = kvp.Key;
                int count = kvp.Value.Count;

                float normalized = (float)count / maxAgentsInAnyCell;
                var color = Color.Lerp(Color.green, Color.red, normalized);
                color.a = 0.23f;
                Gizmos.color = color;

                Vector3 center = new Vector3(
                    (cell.x + 0.5f) * cellSize,
                    (cell.y + 0.5f) * cellSize,
                    0
                );

                Vector3 size = new Vector3(cellSize, cellSize, 0);
                Gizmos.DrawCube(center, size);               // Filled cell
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(center, size);           // Wireframe outline
            }
        }
    }
}