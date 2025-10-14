#if UNITY_EDITOR
using System;
using System.IO;
using InGame;
using InGame.Upgrade.CheatBonus;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;

public class CheatBonusWindow : OdinEditorWindow
{
    // The data you want to edit & serialize
    [Title("Cheat Upgrade Bonus")]
    [InlineProperty, HideLabel]
    [OdinSerialize, NonSerialized] public UpgradeBonusInfo data = new UpgradeBonusInfo();

    [MenuItem("Dark/Cheat/Cheat Bonus Window")]
    private static void Open()
    {
        var w = GetWindow<CheatBonusWindow>();
        w.titleContent = new GUIContent("My Data");
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
            dataAsset.bonus = data;
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
            data = dataAsset.bonus;
        }
        else
        {
            Debug.LogWarning($"File does not exist: {CheatBonusData.FilePath}");
        }
    }
}

#endif
