using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EndlessLevel
{
    [CreateAssetMenu(menuName = "InGame/Level/Level Endless Config", fileName = "LevelEndless")]
    public class LevelEndlessConfig : SerializedScriptableObject
    {
        public int id;
        public WaveEndlessInfo[] waveInfo;
    }
}