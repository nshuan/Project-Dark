using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dark.Scripts.Analytics.PrivateLog
{
    /// <summary>
    /// Logs player stats to a CSV file at Assets/level_analytics.csv
    /// Columns: index, column1, column2 (easily extensible)
    /// </summary>
    public static class LevelAnalyticsLogger
    {
        private const string CSV_FILE_NAME = "level_analytics.csv";
        private const string CSV_HEADER = "index,level,wave,droppedVestige,collectedVestige,collectedExp,totalDmgDealed,totalDmgReceived,detail";
        
        private static string csvFilePath;
        private static int currentIndex = 0;
        private static bool initialized = false;

        /// <summary>
        /// Initialize the logger. Call this once at the start of your game.
        /// </summary>
        public static void Initialize()
        {
            if (initialized) return;
            
#if UNITY_EDITOR
            csvFilePath = Path.Combine(Application.dataPath, CSV_FILE_NAME);
#else
            csvFilePath = Path.Combine(Application.persistentDataPath, CSV_FILE_NAME);
#endif

            // Load existing index if file exists
            if (File.Exists(csvFilePath))
            {
                LoadLastIndex();
            }
            else
            {
                // Create file with header
                WriteHeader();
            }
            
            initialized = true;
        }

        /// <summary>
        /// Log player stats with custom values. Automatically increments index.
        /// Use this method if you've added more columns - just add more parameters.
        /// </summary>
        /// <param name="values">Array of values for each column (excluding index)</param>
        public static void Log(int level, int wave, int droppedVestige, int collectedVestige, int collectedExp, int totalDmgDealed, int totalDmgReceived, string detail)
        {
            if (!initialized) Initialize();
            
            currentIndex++;
            StringBuilder row = new StringBuilder();
            row.Append(currentIndex);
            
            row.Append(',');
            row.Append(EscapeCsvValue(level.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(wave.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(droppedVestige.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(collectedVestige.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(collectedExp.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(totalDmgDealed.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(totalDmgReceived.ToString()));
            
            row.Append(',');
            row.Append(EscapeCsvValue(detail));
            
            AppendRow(row.ToString());
        }
        
        /// <summary>
        /// Log player stats with custom values. Automatically increments index.
        /// Use this method if you've added more columns - just add more parameters.
        /// </summary>
        /// <param name="values">Array of values for each column (excluding index)</param>
        public static void Log(params string[] values)
        {
            if (!initialized) Initialize();
            
            currentIndex++;
            StringBuilder row = new StringBuilder();
            row.Append(currentIndex);
            
            foreach (var value in values)
            {
                row.Append(',');
                row.Append(EscapeCsvValue(value));
            }
            
            AppendRow(row.ToString());
        }

        /// <summary>
        /// Clear all data and reset index. Useful for testing.
        /// </summary>
        public static void Clear()
        {
            currentIndex = 0;
            WriteHeader();
        }

        #region Private Methods

        private static void WriteHeader(string header = CSV_HEADER)
        {
            try
            {
                File.WriteAllText(csvFilePath, header + Environment.NewLine);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"LevelAnalyticsLogger: Failed to write header - {e.Message}");
            }
        }

        private static void AppendRow(string row)
        {
            try
            {
                File.AppendAllText(csvFilePath, row + Environment.NewLine);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"LevelAnalyticsLogger: Failed to append row - {e.Message}");
            }
        }

        private static void LoadLastIndex()
        {
            try
            {
                string[] lines = File.ReadAllLines(csvFilePath);
                if (lines.Length > 1) // Has header + at least one data row
                {
                    // Find the last data row and extract index
                    for (int i = lines.Length - 1; i > 0; i--)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i])) continue;
                        
                        string[] parts = lines[i].Split(',');
                        if (parts.Length > 0 && int.TryParse(parts[0], out int index))
                        {
                            currentIndex = index;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"LevelAnalyticsLogger: Failed to load last index - {e.Message}. Starting from 0.");
                currentIndex = 0;
            }
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // If value contains comma, quote, or newline, wrap in quotes and escape quotes
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        #endregion
    }
}

