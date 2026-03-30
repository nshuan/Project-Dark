// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Core;
// using Dark.Scripts.Common.UIWarning;
// using Dark.Scripts.SceneNavigation;
// using Dark.Scripts.Utils;
// using InGame.EndlessLevel;
// using InGame.GateEditorV2;
// using TMPro;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace InGame.EndlessEditor
// {
//     public class EndlessLevelEditor : MonoSingleton<EndlessLevelEditor>
//     {
// #if UNITY_EDITOR
//         public Camera cam;
//         public RectTransform parentWaves;
//         public EndlessWaveEditor prefabWave;
//         public Button btnSave;
//         public UIPopupWarning popupConfirm;
//         public Button btnPlayLevel;
//
//         [Space] [Header("Display")] 
//         public TextMeshProUGUI txtLevel;
//         
//         public CharacterClass.CharacterClass ClassType { get; set; }
//         
//         private LevelEndlessConfig currentLevel;
//         
//         protected override void Awake()
//         {
//             base.Awake();
//             
//             btnSave.onClick.RemoveAllListeners();
//             btnSave.onClick.AddListener(SaveLevel);
//             btnPlayLevel.onClick.RemoveAllListeners();
//             btnPlayLevel.onClick.AddListener(PlaySelectingLevel);
//         }
//         
//         public void LoadLevel(LevelEndlessConfig level)
//         {
//             currentLevel = level;
//             if (!currentLevel) return;
//             
//             // Destroy all old wave buttons
//             ClearAllWaves();
//             
//             if (currentLevel.waveInfo == null) return;
//             foreach (var waveInfo in currentLevel.waveInfo)
//             {
//                 AddNewWave(waveInfo);
//             }
//             SelectWave(0);
//             
//             txtLevel?.SetText($"Level: {levelId}");
//         }
//
//         public void PlaySelectingLevel()
//         {
//             if (!currentLevel) return;
//             
// #if UNITY_EDITOR
//             LevelManager.isLoadFromInit = true;
// #endif
//             this.DelayCall(0.5f, () =>
//             {
//                 Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
//                 {
//                     LevelManager.Instance.LoadEndlessLevel();
//                 });
//             });
//         }
//
//         #region Save
//
//         public void SaveLevel()
//         {
//             if (!currentLevel) return;
//             foreach (Transform child in parentWaves)
//             {
//                 if (child.TryGetComponent<EndlessWaveEditor>(out var wave))
//                 {
//                     wave.SaveWave();
//                 }
//             }
//
//             currentLevel.mapType = currentMapType;
//             currentLevel.towerPositions = LevelTowerEditorV2.Instance.GetPositions(currentMapType);
//             currentLevel.backgroundIndex = currentBgIndex;
//                         
// #if UNITY_EDITOR
//             EditorUtility.SetDirty(currentLevel);
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
// #endif
//         }
//
//         #endregion
// #endif
//     }
// }