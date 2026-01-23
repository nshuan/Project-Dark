using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level/Level Config", fileName = "Level")]
    public class LevelConfig : SerializedScriptableObject
    {
        public int level; // Start from 1
        public LevelMapType mapType;
        public Vector2[] towerPositions;
        public WaveInfo[] waveInfo;

        private void OnValidate()
        {
            if (waveInfo == null)
            {
                DebugUtility.LogError($"Level {name} does not have any waves!!!");
                return;
            }
            
            for (var i = 0; i < waveInfo.Length; i++)
            {
                waveInfo[i].waveIndex = i;
            }
        }
    }
}