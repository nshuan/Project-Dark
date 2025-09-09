using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Economic;
using InGame.Upgrade;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeImporter
    {
        public static void Import(ScriptableObject[] configs, List<string[]> csvData, Func<string, UpgradeNodeConfig> funcCreateNewNodeConfig, Action callbackCompleteImport = null)
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
                
                if (!configDict.TryGetValue(csvNodeId, out UpgradeNodeConfig config))
                {
                    var nodeName = "Unnamed";
                    for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
                    {
                        if (fields[fieldIndex] != "nodeName") continue;
                        nodeName = cols[fieldIndex];
                        break;
                    }

                    config = funcCreateNewNodeConfig($"{csvNodeId}_{nodeName}");
                    config.nodeId = csvNodeId;
                }
                
                if (costTypeIndexes.Count != costValueIndexes.Count)
                    Debug.LogError("Can't import cost - Number of cost type and cost value do not match");
                else
                {
                    config.costInfo = new UpgradeNodeCostInfo[costTypeIndexes.Count];
                    
                    for (var index = 0; index < costTypeIndexes.Count; index++)
                    {
                        if (!Enum.TryParse<WealthType>(cols[costTypeIndexes[index]], out var costType))
                        {
                            Debug.LogWarning($"Can't import cost - Invalid cost type {cols[costTypeIndexes[index]]}, data index = {i}");
                            continue;
                        }
                        
                        try
                        {
                            var costValue = cols[costValueIndexes[index]].Split(',').Select((str) => int.Parse(str, CultureInfo.InvariantCulture)).ToArray();;

                            var costInfo = new UpgradeNodeCostInfo()
                            {
                                costType = costType,
                                costValue = costValue,
                            };
                            
                            config.costInfo[index] = costInfo;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Can't import cost - Invalid cost value string: {cols[costValueIndexes[index]]}, data index = {i}");
                        }
                    }
                }
                
                // Thêm logic
                var logicInfos = new List<NodeLogicInfo>();
                    
                for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                {
                    ConfigImporter.SetValue(config, fields[fieldIndex], cols[fieldIndex]);
                }
                    
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
                    
                config.nodeLogic = ConfigNodeLogicFactory.Generate(logicInfos);
                EditorUtility.SetDirty(config);
            }
            
            callbackCompleteImport?.Invoke();
        }
        
        public struct LogicImportInfo
        {
            public int typeIndex;
            public List<int> valueIndexes;
            public int isMulIndex;
        }
    }
}