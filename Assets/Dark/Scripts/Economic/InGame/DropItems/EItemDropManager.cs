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
        
        private List<EItemDrop> listItemThisWave;
        private ParticleSystem vfxClaimOnPlayer;
        
        protected override void Awake()
        {
            base.Awake();
            
            listItemThisWave = new List<EItemDrop>();
        }
        
        public void CollectAll()
        {
            if (listItemThisWave.Count == 0) return;

            var delay = 0f;
            var maxDelay = 0f;
            var minDelay = 0f;
            foreach (var item in listItemThisWave)
            {
                delay = Random.Range(0f, 1f);
                if (delay > maxDelay) maxDelay = delay;
                if (delay < minDelay) minDelay = delay;
                item.Collect(LevelManager.Instance.Player.transform, delay);
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
            
            listItemThisWave.Clear();
        }
        
        public void DropOne(WealthType kind, int quantity, Vector3 position, bool canCollectByMouse = false)
        {
            EItemDropPool.Instance.Get(kind, (item) =>
            {
                item.Quantity = quantity;
                item.transform.position = position;
                item.CanCollectByMouse = canCollectByMouse;
                item.CollectDuration = collectDuration;
                item.vfxPositionOffset.x = vfxPositionOffset.x;
                item.vfxPositionOffset.y = vfxPositionOffset.y;
                item.vfxPositionOffset.z = vfxPositionOffset.z;
                listItemThisWave.Add(item);
                item.gameObject.SetActive(true);
                item.Drop(position);
            });
        }
    }
}