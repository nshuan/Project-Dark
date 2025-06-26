using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class CompositeWaveInfo : IWaveInfo
    {
        [field:NonSerialized, OdinSerialize, ReadOnly] public int WaveIndex { get; set; }
        
        [Tooltip("After this time finished, stop all waves that belongs to this composite wave.")]
        public float timeToEnd;
        [NonSerialized, OdinSerialize] public SingleWaveInfo[] subWaves;
        
        public void SetupWave(GateEntity gatePrefab, TowerEntity[] towers)
        {
            foreach (var wave in subWaves)
            {
                wave.SetupWave(gatePrefab, towers);
            }
        }

        public IEnumerator IEActivateWave()
        {
            // Activate các waves
            foreach (var wave in subWaves)
            {
                wave.ActivateWave();
            }
            
            // Sắp xếp waves theo thứ tự tăng dần timeToEnd
            Array.Sort(subWaves, (wave1, wave2) => wave1.timeToEnd.CompareTo(wave2.timeToEnd));

            // Đợi hết timeToEnd của 1 wave thì đợi tiếp khoảng chênh lệch giữa timeToEnd của wave tiếp theo và wave vừa xong
            // Nếu đã đợi qua timeToEnd của nhóm wave thì stop hết
            for (var i = 0; i < subWaves.Length; i++)
            {
                if (i == 0)
                {
                    if (subWaves[i].timeToEnd < timeToEnd) yield return new WaitForSeconds(subWaves[i].timeToEnd);
                    else
                    {
                        yield return new WaitForSeconds(timeToEnd);
                        StopWave();
                        yield break;
                    }
                }
                else
                {
                    if (subWaves[i].timeToEnd < timeToEnd) yield return new WaitForSeconds(subWaves[i].timeToEnd - subWaves[i - 1].timeToEnd);
                    else
                    {
                        yield return new WaitForSeconds(timeToEnd - subWaves[i - 1].timeToEnd);
                        StopWave();
                        yield break;
                    }
                }
                    
                
                subWaves[i].StopWave();
            }
            
            yield return new WaitForSeconds(timeToEnd - subWaves[^1].timeToEnd);
        }

        public void StopWave()
        {
            foreach (var wave in subWaves)
            {
                wave.StopWave();
            }
        }
    }
}