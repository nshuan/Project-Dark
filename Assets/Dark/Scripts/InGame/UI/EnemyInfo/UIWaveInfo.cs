using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InGame.UI.EnemyInfo
{
    public class UIWaveInfo : MonoBehaviour
    {
        [SerializeField] private Transform parentEnemyInfo;
        [SerializeField] private UIEnemyInfo prefabEnemyInfo;
        [SerializeField] private TextMeshProUGUI txtWave;

        private List<UIEnemyInfo> cacheEnemyInfo;
        
        public void UpdateUI(WaveInfo waveInfo)
        {
            cacheEnemyInfo ??= new List<UIEnemyInfo>();

            txtWave.SetText($"Wave {waveInfo.waveIndex}");
            txtWave.gameObject.SetActive(true);
            txtWave.transform.SetAsLastSibling();
            
            var allEnemy = new Dictionary<EnemyBehaviour, int>(); // <Enemy config, amount>
            foreach (var gate in waveInfo.waveConfig.gateConfigs)
            {
                var gateSpawn = gate.duration >= 0 ? (int)(gate.duration / gate.intervalLoop) + 1 : -1;

                if (gateSpawn == -1) allEnemy[gate.spawnType] = gateSpawn;
                else 
                {
                    if (allEnemy.TryGetValue(gate.spawnType, out var totalSpawn))
                    {
                        if (totalSpawn >= 0) totalSpawn += gateSpawn;
                    }
                    else totalSpawn = gateSpawn;
                    
                    allEnemy[gate.spawnType] = totalSpawn;
                }
            }

            var infoIndex = 0;
            foreach (var pair in allEnemy)
            {
                UIEnemyInfo newInfo = null;
                if (infoIndex >= cacheEnemyInfo.Count)
                {
                    newInfo = Instantiate(prefabEnemyInfo, parentEnemyInfo);
                    cacheEnemyInfo.Add(newInfo);
                }
                else
                {
                    newInfo = cacheEnemyInfo[infoIndex];
                }
                
                newInfo.UpdateUI(
                    pair.Key.displayName,
                    LevelUtilityV2.ToInt(pair.Key.hp * waveInfo.scaleHp),
                    LevelUtilityV2.ToInt(pair.Key.dmg * waveInfo.scaleDmg),
                    pair.Key.moveSpeed * waveInfo.scaleSpeed,
                    pair.Value);

                newInfo.gameObject.SetActive(true);
                newInfo.transform.SetAsLastSibling();
                
                infoIndex += 1;
            }

            for (var i = infoIndex; i < cacheEnemyInfo.Count; i++)
            {
                cacheEnemyInfo[i].gameObject.SetActive(false);
            }
        }
    }
}