using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EndlessLevel
{
    [CreateAssetMenu(menuName = "InGame/Level/Wave Endless Pool", fileName = "WaveEndlessPool")]
    public class PoolWaveEndless : SerializedScriptableObject
    {
        public WaveEndlessType waveType;
        public WaveEndlessConfig[] allWaves;
    }
}