using System;
using System.Collections;

namespace InGame
{
    public interface IWaveInfo
    {
        int WaveIndex { get; set; }
        bool WaveEndedCompletely { get; }
        void SetupWave(GateEntity gatePrefab, TowerEntity[] towers, Action onWaveForceEnded);
        IEnumerator IEActivateWave();
        void StopWave();
    }
}