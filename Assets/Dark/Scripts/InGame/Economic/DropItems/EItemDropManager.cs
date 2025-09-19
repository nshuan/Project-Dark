using System.Collections.Generic;
using Core;
using Economic;
using InGame.UI.Economic.KillShowCollected;
using UnityEngine;

namespace InGame.Economic.DropItems
{
    public class EItemDropManager : MonoSingleton<EItemDropManager>
    {
        private List<EItemDrop> listItemThisWave;
        
        protected override void Awake()
        {
            base.Awake();
            
            listItemThisWave = new List<EItemDrop>();
            
            LevelManager.Instance.onWaveEnded += OnWaveEnded;
        }
        
        private void OnWaveEnded(int waveIndex)
        {
            if (listItemThisWave.Count == 0) return;

            foreach (var item in listItemThisWave)
            {
                if (item.Quantity > 0)
                {
                    switch (item.kind)
                    {
                        case WealthType.Vestige:
                            WealthManager.Instance.AddDark(item.Quantity);
                            break;
                        case WealthType.Echoes:
                            WealthManager.Instance.AddLevelPoint(item.Quantity);
                            break;
                        case WealthType.Sigils:
                            WealthManager.Instance.AddBossPoint(item.Quantity);
                            break;
                    }
                }
                
                item.Collect(() =>
                {
                    UIKillCollectedPool.Instance.ShowCollected(item.kind, item.Quantity, item.transform.position);
                    EItemDropPool.Instance.Release(item);
                });
            }
            
            listItemThisWave.Clear();
        }
        
        public void DropOne(WealthType kind, int quantity, Vector3 position)
        {
            EItemDropPool.Instance.Get(kind, (item) =>
            {
                item.Quantity = quantity;
                item.transform.position = position;
                listItemThisWave.Add(item);
                item.gameObject.SetActive(true);
                item.Drop();
            });
        }
    }
}