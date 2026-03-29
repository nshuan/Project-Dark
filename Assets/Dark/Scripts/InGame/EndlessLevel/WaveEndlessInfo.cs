using System;

namespace InGame.EndlessLevel
{
    [Serializable]
    public class WaveEndlessInfo
    {
        public float scaleHp = 1f;
        public float scaleDmg = 1f;
        public float scaleSpe = 1f;
        public float expRatio = 1f;
        public float darkRatio = 1f;
        public int darkUnitValue = 1;
        public int sigils = 0;
        public int ashes = 0;
        public PoolWaveEndless wavePool;
    }
}