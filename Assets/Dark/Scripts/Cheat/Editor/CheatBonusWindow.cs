#if UNITY_EDITOR
using System;
using System.IO;
using InGame;
using Cheat;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;

public class CheatBonusWindow : OdinEditorWindow
{
    // The data you want to edit & serialize
    [Title("Cheat Upgrade Bonus")] 
    [OdinSerialize, NonSerialized] public bool enable;
    [InlineProperty, HideLabel] 
    [OdinSerialize, NonSerialized] public UpgradeBonusInfo data = new UpgradeBonusInfo();

    [MenuItem("Dark/Cheat/Cheat Bonus Window")]
    private static void Open()
    {
        var w = GetWindow<CheatBonusWindow>();
        w.titleContent = new GUIContent("Cheat Bonus");
        w.minSize = new Vector2(500, 350);
        w.Refresh();
        w.Show();
    }
    
    [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
    private void Save()
    {
        if (File.Exists(CheatBonusData.FilePath))
        {
            var dataAsset = AssetDatabase.LoadAssetAtPath<CheatBonusData>(CheatBonusData.FilePath);
            dataAsset.enabled = enable;
            dataAsset.bonus = data;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(dataAsset);
        }
        else
        {
            Debug.LogWarning($"File does not exist: {CheatBonusData.FilePath}");
        }
    }

    [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
    public void Refresh()
    {
        if (File.Exists(CheatBonusData.FilePath))
        {
            var dataAsset = AssetDatabase.LoadAssetAtPath<CheatBonusData>(CheatBonusData.FilePath);
            enable = dataAsset.enabled;
            data = dataAsset.bonus;
        }
        else
        {
            Debug.LogWarning($"File does not exist: {CheatBonusData.FilePath}");
        }
    }
}

#endif
