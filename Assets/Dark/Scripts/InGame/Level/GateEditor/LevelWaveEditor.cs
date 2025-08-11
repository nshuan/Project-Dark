using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.GateEditor
{
    public class LevelWaveEditor : MonoBehaviour
    {
        [SerializeField] private LevelGateConfigEditor gateEditorPrefab;
        
        public WaveConfig WaveConfig { get; set; }

        public void LoadWaves(WaveInfo waveInfo)
        {
            WaveConfig = waveInfo.waveConfig;
            name = "Wave " + waveInfo.waveIndex;
            
            // Delete all child
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            
            if (waveInfo.waveConfig.gateConfigs == null)
            {
                DebugUtility.LogError($"{name} has no gates!");
                return;
            }
            
            foreach (var gateConfig in waveInfo.waveConfig.gateConfigs)
            {
                var newGate = Instantiate(gateEditorPrefab, transform);
                newGate.SetupGate(gateConfig);
            }
        }
        
        private bool IsWaveInfoNull => WaveConfig == null;
        [Button, DisableIf("IsWaveInfoNull")]
        public void AddNewGate()
        {
            var newGateConfig = new GateConfig();
            var newGate = Instantiate(gateEditorPrefab, transform);
            newGate.SetupGate(newGateConfig);
            
#if UNITY_EDITOR
            
            // Select it in the hierarchy
            Selection.activeGameObject = newGate.gameObject;

            // Ping it to highlight in inspector/hierarchy
            EditorGUIUtility.PingObject(newGate.gameObject);

            // Focus Scene View (optional)
            SceneView.lastActiveSceneView.FrameSelected();
            
#endif
        }

        [Button]
        public void SaveWave()
        {
            if (WaveConfig == null) return;

            WaveConfig.gateConfigs = new List<GateConfig>();
            foreach (Transform gate in transform)
            {
                if (gate.TryGetComponent<LevelGateConfigEditor>(out var gateConfigEditor))
                {
                    WaveConfig.gateConfigs.Add(new GateConfig()
                    {
                        position = gate.position,
                        targetBaseIndex = gateConfigEditor.gateConfig.targetBaseIndex,
                        startTime = gateConfigEditor.gateConfig.startTime,
                        duration = gateConfigEditor.gateConfig.duration,
                        spawnType = gateConfigEditor.gateConfig.spawnType,
                        intervalLoop = gateConfigEditor.gateConfig.intervalLoop,
                        spawnLogic = gateConfigEditor.gateConfig.spawnLogic,
                    });
                }
            }
        }
    }
}