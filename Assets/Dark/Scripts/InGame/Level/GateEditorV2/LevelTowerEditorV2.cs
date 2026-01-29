using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame.GateEditorV2
{
    public class LevelTowerEditorV2 : SerializedMonoSingleton<LevelTowerEditorV2>
    {
        public Dictionary<LevelMapType, TowerEntity[]> towersMap;

        public void SetPosition(LevelMapType mapType, Vector2[] positions)
        {
            if (towersMap == null || towersMap.Count == 0) return;
            if (!towersMap.TryGetValue(mapType, out var towers)) return;
            if (positions == null || positions.Length == 0) return;
            
            for (var i = 0; i < positions.Length; i++)
            {
                if (i < towers.Length) towers[i].transform.position = positions[i];    
            }
        }

        public Vector2[] GetPositions(LevelMapType mapType)
        {
            if (towersMap == null || towersMap.Count == 0) return Array.Empty<Vector2>();
            if (!towersMap.TryGetValue(mapType, out var towers)) return Array.Empty<Vector2>();
            
            var positions = new Vector2[towers.Length];
            for (var i = 0; i < towers.Length; i++)
            {
                positions[i] = towers[i].transform.position;
            }

            return positions;
        }

        public TowerEntity[] GetTowers(LevelMapType mapType)
        {
            if (towersMap == null || towersMap.Count == 0) return Array.Empty<TowerEntity>();
            if (towersMap.TryGetValue(mapType, out var towers)) return towers;
            return Array.Empty<TowerEntity>();
        }
    }
}