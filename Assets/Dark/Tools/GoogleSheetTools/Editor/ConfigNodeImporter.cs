using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeImporter
    {
        public static void Import(ScriptableObject[] configs, List<string[]> csvData)
        {
            var configDict = new Dictionary<int, UpgradeNodeConfig>();
            foreach (var config in configs)
            {
                if (config is not UpgradeNodeConfig upgradeConfig)
                {
                    Debug.LogError($"Config: {config.name} is not UpgradeNodeConfig");
                    continue;
                }
                
                // Validate configs name
                var underscoreIndex = upgradeConfig.name.IndexOf('_');
                if (underscoreIndex <= 0)
                {
                    Debug.LogError($"Invalid config name: {upgradeConfig.name}");
                    continue;
                }

                // Validate config index
                if (int.TryParse(upgradeConfig.name.Substring(0, underscoreIndex), out var configId))
                {
                    configDict.Add(configId, upgradeConfig);
                }
                else
                {
                    Debug.LogError($"Invalid config id: {upgradeConfig.name}");
                    return;
                }
                
                configDict.TryAdd(configId, upgradeConfig);
            }
            
            // Header is field names
            var fields = csvData[0];
            var costTypeIndexes = new List<int>();
            var costValueIndexes = new List<int>();
            var logicIndexes = new List<LogicImportInfo>();

            var isLogicCols = false;
            LogicImportInfo tempLogicImport = new LogicImportInfo();
            
            // Get logics
            for (var i = 0; i < fields.Length; i++)
            {
                if (!isLogicCols)
                {
                    if (fields[i].ToLower().Contains("cost_type"))
                        costTypeIndexes.Add(i);
                    if (fields[i].ToLower().Contains("cost_value"))
                        costValueIndexes.Add(i);
                    if (fields[i].ToLower().Contains("logic_type"))
                    {
                        isLogicCols = true;
                        tempLogicImport = new LogicImportInfo()
                        {
                            typeIndex = i,
                            valueIndexes = new List<int>()
                        };
                    }
                }
                else
                {
                    if (fields[i].ToLower().Contains("logic_value"))
                        tempLogicImport.valueIndexes.Add(i);
                    if (fields[i].ToLower().Contains("logic_is_mul"))
                    {
                        tempLogicImport.isMulIndex = i;
                        isLogicCols = false;
                        logicIndexes.Add(tempLogicImport);
                    }
                }
            }

            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;
                
                if (!int.TryParse(cols[0], out var csvNodeId))
                {
                    Debug.LogWarning($"Row {i} skipped — invalid nodeId");
                    continue;
                }
                
                // Todo Thêm cost
                
                // Thêm logic
                if (configDict.TryGetValue(csvNodeId, out UpgradeNodeConfig config))
                {
                    var logicInfos = new List<NodeLogicInfo>();
                    
                    foreach (var indexInfo in logicIndexes)
                    {
                        logicInfos.Add(new NodeLogicInfo()
                        {
                            key = cols[indexInfo.typeIndex],
                            value = indexInfo.valueIndexes.Select((index) => cols[index]).ToList(),
                            isMul = cols[indexInfo.isMulIndex]
                        });
                    }

                    var a = 1;
                    
                    // config.nodeLogic = ConfigNodeLogicGenerator.Generate(logicInfos);
                    // EditorUtility.SetDirty(config);
                }
                else
                {
                    Debug.LogError($"No matching config found for nodeId {csvNodeId}");
                }
            }
        }
        
        public struct LogicImportInfo
        {
            public int typeIndex;
            public List<int> valueIndexes;
            public int isMulIndex;
        }
    }
}