using System;
using System.Collections.Generic;
using System.Globalization;
using InGame;
using UnityEditor;
using UnityEngine;

namespace GoogleSheetTool
{
    public class EnemyConfigImporter : ConfigImporter
    {
        public override void Import(ScriptableObject[] configs, List<string[]> csvData)
        {
            var enemyDict = new Dictionary<int, EnemyBehaviour>();
            foreach (var config in configs)
            {
                if (config is EnemyBehaviour enemyConfig)
                    enemyDict.TryAdd(enemyConfig.enemyId, enemyConfig);
                else
                    Debug.LogWarning($"Config {config.name} is not EnemyBehaviour");
            }
            
            // Header is field names
            var fields = csvData[0];
            
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;
                
                if (!int.TryParse(cols[0], out var csvEnemyId))
                {
                    Debug.LogWarning($"Row {i+1} skipped — invalid enemyId");
                    continue;
                }

                if (enemyDict.TryGetValue(csvEnemyId, out EnemyBehaviour config))
                {
                    for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                    {
                        SetValue(config, fields[fieldIndex], cols[fieldIndex]);
                    }
                    
                    // config.attackRange        = float.Parse(cols[1], CultureInfo.InvariantCulture);
                    // config.attackSpeed        = float.Parse(cols[2], CultureInfo.InvariantCulture);
                    // config.hp                 = int.Parse(cols[3]);
                    // config.dmg                = int.Parse(cols[4]);
                    // config.moveSpeed          = float.Parse(cols[5], CultureInfo.InvariantCulture);
                    // config.staggerResist      = float.Parse(cols[6], CultureInfo.InvariantCulture);
                    // config.staggerMaxDuration = float.Parse(cols[7], CultureInfo.InvariantCulture);
                    // config.invisibleDuration  = float.Parse(cols[8], CultureInfo.InvariantCulture);
                    // config.exp                = int.Parse(cols[9]);
                    // config.dark               = int.Parse(cols[10]);
                    // config.darkRatio          = float.Parse(cols[11], CultureInfo.InvariantCulture);
                    // config.bossPoint          = int.Parse(cols[12]);

                    EditorUtility.SetDirty(config);
                }
                else
                {
                    Debug.LogError($"No matching EnemyBehaviour found for enemyId {csvEnemyId}");
                }
            }
        }
    }
}