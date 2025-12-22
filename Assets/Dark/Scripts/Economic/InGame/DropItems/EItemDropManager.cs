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
        public ECollectorData CollectedData => collectedData;
        
        private List<EItemDrop> listItemToCollect;
        private ParticleSystem vfxClaimOnPlayer;
        
        protected override void Awake()
        {
            base.Awake();
            
            listItemToCollect = new List<EItemDrop>();
            collectedData = new ECollectorData();

            WealthManager.Instance.OnLevelPointChanged += OnEchoesAdded;
        }

        protected override void OnDestroy()
        {
            WealthManager.Instance.OnLevelPointChanged -= OnEchoesAdded;
            
            base.OnDestroy();
        }

        public void CollectAll(Transform target)
        {
            if (listItemToCollect.Count == 0)
            {
                CombatActions.OnCollectAllResourceDrop?.Invoke(0, 0f);
                return;
            }
            
            var delay = 0f;
            var maxDelay = 0f;
            var minDelay = 0f;
            var index = 0;
            foreach (var item in listItemToCollect)
            {
                switch (item.kind)
                {
                    case WealthType.Vestige:
                        collectedData.AddVestige(item.Quantity);
                        break;
                    case WealthType.Sigils:
                        collectedData.AddSigils(item.Quantity);
                        break;
                }
                
                // 5 cái đầu ko nên delay random
                if (index < 5)
                    delay = 0.02f * index;
                else
                    delay = RandomUtil.Range(0f, 0.2f);
                
                if (delay > maxDelay) maxDelay = delay;
                if (delay < minDelay) minDelay = delay;
                    
                item.Collect(target, delay);

                index += 1;
            }
            
            CombatActions.OnCollectAllResourceDrop?.Invoke(listItemToCollect.Count, maxDelay + 0.2f);
            
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
        
        private void OnEchoesAdded(int before, int after)
        {
            collectedData.AddEchoes(after - before);
        }
    }
    
    public class ECollectorData
    {
        public int TotalCollectedVestige { get; private set; }
        public int TotalCollectedEchoes { get; private set; }
        public int TotalCollectedSigils { get; private set; }
        public int Vestige { get; private set; }
        public int Sigils { get; private set; }
        public int Echoes { get; private set; }

        public void Claim()
        {
            if (Vestige > 0)  WealthManager.Instance.AddVestige(Vestige);
            if (Sigils > 0)  WealthManager.Instance.AddBossPoint(Sigils);
            
            // Echoes không collect ở đây, khi đủ exp đã tự + echoes rồi

            Vestige = 0;
            Sigils = 0;
            Echoes = 0;
        }

        public void AddVestige(int amount)
        {
            Vestige += amount;
            TotalCollectedVestige += amount;
        }

        public void AddEchoes(int amount)
        {
            Echoes += amount;
            TotalCollectedEchoes += amount;
        }

        public void AddSigils(int amount)
        {
            Sigils += amount;
            TotalCollectedSigils += amount;
        }
    }
}