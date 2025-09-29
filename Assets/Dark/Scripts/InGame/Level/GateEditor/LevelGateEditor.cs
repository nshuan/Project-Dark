using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.GateEditor
{
    public class LevelGateEditor : SerializedMonoBehaviour
    {
        [SerializeField] private LevelWaveEditor waveEditorPrefab;
        
        [Space]
        [SerializeField] private LevelConfig level;
        
        public List<LevelWaveEditor> waves = new List<LevelWaveEditor>();

        [Button]
        public void Loadlevel()
        {
            // Destroy all child
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            
            waves = new List<LevelWaveEditor>();
            if (level == null)
            {
                DebugUtility.LogError("Level has no waves!");
                return;
            }

            foreach (var wave in level.waveInfo)
            {
                var newWaveEditor = Instantiate(waveEditorPrefab, transform);
                newWaveEditor.LoadWaves(wave);
                waves.Add(newWaveEditor);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        [Button]
        public void SaveLevel()
        {
            foreach (var wave in waves)
            {
                wave.SaveWave();
            }
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(level);
#endif
        }
    }
}