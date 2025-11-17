using System.Collections.Generic;
using Core;
using Dark.Scripts.Utils;
using InGame;
using UnityEngine;

namespace Economic.InGame.DropItems
{
    public class EItemDropManager : MonoSingleton<EItemDropManager>
    {
        [SerializeField] private ParticleSystem vfxClaimOnPlayerPrefab;
        
        [Space]
        [Header("Config")]
        [SerializeField] private float collectDuration = 0.7f;

        [SerializeField] private Vector3 vfxPositionOffset = new Vector3(-0.11f, -0.45f, 0f); 
        
        private ECollectorData collectedData;
        
        private List<EItemDrop> listItemToCollect;
        private ParticleSystem vfxClaimOnPlayer;
        
        protected override void Awake()
        {
            base.Awake();
            
            listItemToCollect = new List<EItemDrop>();
            collectedData = new ECollectorData();
        }
        
        public void CollectAll(Transform target)
        {
            if (listItemToCollect.Count == 0) return;
            
            var delay = 0f;
            var maxDelay = 0f;
            var minDelay = 0f;
            foreach (var item in listItemToCollect)
            {
                switch (item.kind)
                {
                    case WealthType.Vestige:
                        collectedData.vestige += item.Quantity;
                        break;
                    case WealthType.Echoes:
                        collectedData.echoes += item.Quantity;
                        break;
                    case WealthType.Sigils:
                        collectedData.sigils += item.Quantity;
                        break;
                }
                
                delay = RandomUtil.Range(0f, 1f);
                if (delay > maxDelay) maxDelay = delay;
                if (delay < minDelay) minDelay = delay;
                item.Collect(target, delay);
            }
            
            this.DelayCall(minDelay + collectDuration, () =>
            {
                vfxClaimOnPlayer ??= Instantiate(vfxClaimOnPlayerPrefab);
                vfxClaimOnPlayer.gameObject.SetActive(false);
                vfxClaimOnPlayer.transform.position = LevelManager.Instance.Player.transform.position + vfxPositionOffset;
                var main = vfxClaimOnPlayer.main;
                main.duration = maxDelay - minDelay;
                vfxClaimOnPlayer.gameObject.SetActive(true);
                vfxClaimOnPlayer.Play();
            });
            
            collectedData.Claim();
            
            listItemToCollect.Clear();
        }
        
        public void DropOne(WealthType kind, int quantity, Vector3 position)
        {
            EItemDropPool.Instance.Get(kind, (item) =>
            {
                item.Quantity = quantity;
                item.transform.position = position;
                item.vfxPositionOffset.x = vfxPositionOffset.x;
                item.vfxPositionOffset.y = vfxPositionOffset.y;
                item.vfxPositionOffset.z = vfxPositionOffset.z;
                listItemToCollect.Add(item);
                item.gameObject.SetActive(true);
                item.Drop(position);
            });
        }

        public void Drop(WealthType kind, int quantity, int amount, Vector3 position)
        {
            for (var i = 0; i < amount; i++)
            {
                DropOne(kind, quantity, position);
            }
        }
    }
    
    public class ECollectorData
    {
        public int vestige;
        public int sigils;
        public int echoes;

        public void Claim()
        {
            if (vestige > 0)  WealthManager.Instance.AddDark(vestige);
            if (sigils > 0)  WealthManager.Instance.AddBossPoint(sigils);
            if (echoes > 0)  WealthManager.Instance.AddLevelPoint(echoes);

            vestige = 0;
            sigils = 0;
            echoes = 0;
        }
    }
}