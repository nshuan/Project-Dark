using System.Collections.Generic;
using Dark.Scripts.InGame.Upgrade;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeCostImporter
    {
        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (config is not UpgradeRequirementConfig costConfig)
            {
                Debug.LogError($"Invalid requirement config!");
                return;
            }
            
            // Header is field names
            var fields = csvData[0];

            var costMap = new Dictionary<int, (int, int, int)>();
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                if (int.TryParse(cols[0], out var resultId))
                {
                    if (costMap.ContainsKey(resultId))
                    {
                        Debug.LogError($"There are 2 line with the same id {resultId}, line {i}");
                        continue;
                    }

                    var vestige = 0;
                    var echoes = 0;
                    var sigils = 0;
                    
                    if (cols.Length > 1 && int.TryParse(cols[1], out var resultVestige))
                    {
                        vestige = resultVestige;
                    }
                    else
                    {
                        vestige = 0;
                        Debug.LogError($"Invalid vestige value at line {i}");
                    }
                        
                    if (cols.Length > 2 && int.TryParse(cols[2], out var resultEchoes))
                    {
                        echoes = resultEchoes;
                    }
                    else
                    {
                        echoes = 0;
                        Debug.LogError($"Invalid echoes value at line {i}");
                    }
                    
                    if (cols.Length > 3 && int.TryParse(cols[3], out var resultSigils))
                    {
                        sigils = resultSigils;
                    }
                    else
                    {
                        sigils = 0;
                        Debug.LogError($"Invalid sigils value at line {i}");
                    }
                    
                    costMap.Add(resultId, (vestige, echoes, sigils));
                }
                else
                {
                    Debug.LogError($"Invalid Id {cols[0]} at line {i}");
                    continue;
                }
            }
            
            costConfig.requirementInfos = new List<UpgradeRequirementInfo>();
            foreach (var cost in costMap)
            {
                costConfig.requirementInfos.Add(new UpgradeRequirementInfo()
                {
                    index = cost.Key,
                    vestige = cost.Value.Item1,
                    echoes = cost.Value.Item2,
                    sigils = cost.Value.Item3
                });
            }
            costConfig.SortByIndexAscending();

#if UNITY_EDITOR
            EditorUtility.SetDirty(costConfig);

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = costConfig;
#endif
        }
    }
}