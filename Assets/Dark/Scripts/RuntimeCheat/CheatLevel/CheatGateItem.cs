using System;
using System.Collections.Generic;
using System.Linq;
using InGame;
using InGame.Pause;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.RuntimeCheat.CheatLevel
{
    public class CheatGateItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action ActionOpenConfigBoard { get; set; }
        public Action ActionStartMove { get; set; }
        public Action ActionEndMove { get; set; }

        public List<CheatGateConfigInfo> info;
        
        private bool dragging;

        private void Awake()
        {
            PauseGame.Instance.onPause += OnPause;
        }

        private void OnPause(bool isPause)
        {
            if (isPause) return;
            
            if (info == null) return;

            var maxDuration = 0f;
            CheatGateConfigInfo infoWithMaxDuration = null;

            foreach (var gateInfo in info)
            {
                if (gateInfo.amount * gateInfo.interval > maxDuration)
                {
                    maxDuration = gateInfo.amount * gateInfo.interval;
                    infoWithMaxDuration = gateInfo;
                }
            }
            
            foreach (var gateInfo in info)
            {
                if (gateInfo == null) continue;
                var durationVisual = ReferenceEquals(gateInfo, infoWithMaxDuration)
                    ? gateInfo.amount * gateInfo.interval
                    : 0f;
                var newGateConfig = new GateConfig()
                {
                    isBossGate = false,
                    position = VisualEffectHelper.Instance.DefaultCamera.ScreenToWorldPoint(transform.position),
                    intervalLoop = gateInfo.interval,
                    spawnLogic = new GateSpawnSingle(),
                    targetBaseIndex = new int[1] { gateInfo.targetTowerId },
                    spawnType = gateInfo.enemyConfig,
                    duration = gateInfo.amount * gateInfo.interval,
                    durationVisual = durationVisual,
                };
                
                var gate = Instantiate(LevelManager.Instance.gatePrefab, newGateConfig.position, quaternion.identity, null);
                gate.Initialize(newGateConfig, newGateConfig.targetBaseIndex.Select((index) => LevelManager.Instance.Towers[index]).ToArray(), new WaveStatsScale(), 1f, 1f, 1);
                gate.Activate();
            }
            
            gameObject.SetActive(false);
        }

        public void SetDragging(bool dragging)
        {
            this.dragging = dragging;
        }

        private void Update()
        {
            if (!dragging) return;
            transform.position = Input.mousePosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ActionOpenConfigBoard?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragging = true;
            ActionStartMove?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
            ActionEndMove?.Invoke();
        }
    }
}