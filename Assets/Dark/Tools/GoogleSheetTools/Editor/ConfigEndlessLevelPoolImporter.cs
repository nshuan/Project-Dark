using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using InGame.EndlessLevel;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigEndlessLevelPoolImporter
    {
        static NumberFormatInfo format = new NumberFormatInfo { NumberDecimalSeparator = "," };

        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (!config) return;
            if (config is not PoolWaveEndless poolConfig) return;

            var pool = new Dictionary<int, Dictionary<WaveEndlessType, List<int>>>();
            pool.Add(0, new Dictionary<WaveEndlessType, List<int>>());
            pool.Add(1, new Dictionary<WaveEndlessType, List<int>>());
            pool.Add(2, new Dictionary<WaveEndlessType, List<int>>());
            var maxWaveType = Enum.GetNames(typeof(WaveEndlessType)).Length;
            
            // Header is field names
            var fields = csvData[0];
            
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                for (var colIndex = 0; colIndex < cols.Length; colIndex++)
                {
                    var col = cols[colIndex];
                    if (!int.TryParse(col, out var colValue))
                        continue;
                    
                    var colName = fields[colIndex];
                    var parts = colName.Split("_");
                    if (!int.TryParse(parts[1], out var mapIndex)) continue;
                    if (!int.TryParse(parts[3], out var waveTypeIndex) || waveTypeIndex >= maxWaveType) continue;
                    if (!pool.TryGetValue(mapIndex, out var mapDict)) continue;
                    var waveType = (WaveEndlessType)waveTypeIndex;
                    if (!mapDict.ContainsKey(waveType)) mapDict.Add(waveType, new List<int>());
                    mapDict[waveType].Add(colValue);
                }
            }

            poolConfig.pool = pool;
            EditorUtility.SetDirty(poolConfig);
        }
    }
}