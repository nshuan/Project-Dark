using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetUtility
{
    public static List<T> LoadAllScriptableObjectsInFolder<T>(string folderPath) where T : ScriptableObject
    {
        List<T> results = new List<T>();

        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
                results.Add(asset);
        }

        return results;
    }
}
