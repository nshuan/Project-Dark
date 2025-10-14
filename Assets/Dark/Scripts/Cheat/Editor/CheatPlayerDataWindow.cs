#if UNITY_EDITOR
using System;
using System.IO;
using InGame;
using Cheat;
using Data;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;

public class CheatPlayerDataWindow : OdinEditorWindow
{
    // The data you want to edit & serialize
    [Title("Cheat Player Data Slot 1")]
    [HorizontalLine(2f, 8f, 0.1f, 0.5f, 1f)]
    [InlineProperty, HideLabel]
    [OdinSerialize, NonSerialized] public PlayerData dataSlot1 = new PlayerData();
    
    
    [Title("Cheat Player Data Slot 2")]
    [HorizontalLine(2f, 8f, 0.1f, 0.5f, 1f)]
    [InlineProperty, HideLabel]
    [OdinSerialize, NonSerialized] public PlayerData dataSlot2 = new PlayerData();
    
    [Title("Cheat Player Data Slot 3")]
    [HorizontalLine(2f, 8f, 0.1f, 0.5f, 1f)]
    [InlineProperty, HideLabel]
    [OdinSerialize, NonSerialized] public PlayerData dataSlot3 = new PlayerData();
    
    [Title("Cheat Player Data Slot 4")]
    [HorizontalLine(2f, 8f, 0.1f, 0.5f, 1f)]
    [InlineProperty, HideLabel]
    [OdinSerialize, NonSerialized] public PlayerData dataSlot4 = new PlayerData();

    [MenuItem("Dark/Cheat/Cheat Player Data")]
    private static void Open()
    {
        var w = GetWindow<CheatPlayerDataWindow>();
        w.titleContent = new GUIContent("Cheat Player Data");
        w.minSize = new Vector2(500, 350);
        w.Refresh();
        w.Show();
    }
    
    [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
    private void Save()
    {
        // Slot 1
        var dataPath = GetDataPath(1);
        var jsonData = "";
        if (dataSlot1 != null)
        {
            jsonData = JsonUtility.ToJson(dataSlot1);
            File.WriteAllText(dataPath, jsonData);
        }
        
        // Slot 2
        dataPath = GetDataPath(2);
        if (dataSlot2 != null)
        {
            jsonData = JsonUtility.ToJson(dataSlot2);
            File.WriteAllText(dataPath, jsonData);
        }
        
        // Slot 3
        dataPath = GetDataPath(3);
        if (dataSlot3 != null)
        {
            jsonData = JsonUtility.ToJson(dataSlot3);
            File.WriteAllText(dataPath, jsonData);
        }
        
        // Slot 4
        dataPath = GetDataPath(4);
        if (dataSlot4 != null)
        {
            jsonData = JsonUtility.ToJson(dataSlot4);
            File.WriteAllText(dataPath, jsonData);
        }
        
        AssetDatabase.Refresh();
    }

    [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
    public void Refresh()
    {
        // Slot 1
        var dataPath = GetDataPath(1);
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            dataSlot1 = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            dataSlot1 = null;
        }
        
        // Slot 2
        dataPath = GetDataPath(2);
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            dataSlot2 = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            dataSlot2 = null;
        }
        
        // Slot 3
        dataPath = GetDataPath(3);
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            dataSlot3 = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            dataSlot3 = null;
        }
        
        // Slot 4
        dataPath = GetDataPath(4);
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            dataSlot4 = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            dataSlot4 = null;
        }
    }

    [HorizontalGroup("Buttons"), Button(ButtonSizes.Small)]
    public void ClearData()
    {
        if (EditorUtility.DisplayDialog(
                "Confirm Action",                   // Title
                "Are you sure you want to clear data of ALL SLOTS?", // Message
                "Yes",                               // OK button
                "No"))                               // Cancel button
        {
            // User clicked "Yes"
            
            // Slot 1
            var dataPath = GetDataPath(1);
            dataSlot1 = null;
            File.Delete(dataPath);
        
            // Slot 2
            dataPath = GetDataPath(2);
            dataSlot2 = null;
            File.Delete(dataPath);
        
            // Slot 3
            dataPath = GetDataPath(3);
            dataSlot3 = null;
            File.Delete(dataPath);
        
            // Slot 4
            dataPath = GetDataPath(4);
            dataSlot4 = null;
            File.Delete(dataPath);
            
            AssetDatabase.Refresh();
            
            Debug.Log("Data cleared!");
        }
        else
        {
            // User clicked "No"
            Debug.Log("Action canceled!");
        }
    }

    private string GetDataPath(int slot)
    {
        switch (slot)
        {
            case 1: return DataHandler.DataPath + "/playerDataSlot0.json";
            case 2: return DataHandler.DataPath + "/playerDataSlot1.json";
            case 3: return DataHandler.DataPath + "/playerDataSlot2.json";
            case 4: return DataHandler.DataPath + "/playerDataSlot3.json";
        }

        return "";
    }
}

#endif
