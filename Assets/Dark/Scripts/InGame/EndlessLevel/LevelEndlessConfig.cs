using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EndlessLevel
{
    [CreateAssetMenu(menuName = "InGame/Level/Level Endless Config", fileName = "LevelEndless")]
    public class LevelEndlessConfig : SerializedScriptableObject
    {
        public int id;
        public Dictionary<int, Vector2[]> towerPositionsMap;
        public WaveEndlessInfo[] waveInfo;
    }
}