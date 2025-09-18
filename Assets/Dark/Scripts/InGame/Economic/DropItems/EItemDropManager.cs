using System.Collections.Generic;
using Core;
using Economic;
using UnityEngine;

namespace InGame.Economic.DropItems
{
    public class EItemDropManager : MonoSingleton<EItemDropManager>
    {
        private List<EItemDrop> listItemThisWave;
        
        protected override void Awake()
        {
            base.Awake();

            LevelManager.Instance.onWaveEnded += OnWaveEnded;
        }

        private void OnWaveEnded(int waveIndex)
        {
            
        }
        
        public void DropOne(WealthType kind, int quantity, Vector3 position)
        {
            
        }
    }
}