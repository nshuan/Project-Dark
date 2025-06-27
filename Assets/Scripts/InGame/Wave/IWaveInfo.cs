using System;
using System.Collections;

namespace InGame
{
    public interface IWaveInfo
    {
        int WaveIndex { get; set; }
        void SetupWave(GateEntity gatePrefab, TowerEntity[] towers, Action onWaveForceEnded);
        IEnumerator IEActivateWave();
        void StopWave();
    }
}