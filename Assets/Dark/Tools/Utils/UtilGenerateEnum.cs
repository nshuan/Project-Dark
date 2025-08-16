using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace Dark.Tools.Utils
{
#if UNITY_EDITOR
    public class UtilGenerateEnum
    {
        public static void GenerateEnumScript(string path,  string enumName, List<string> rawNames, List<int> values = null,
            string spacename = "Dark.Tools")
        {
            var code = GenerateEnumCode(enumName, rawNames, values, spacename);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, code, new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.Log($"Generated enum '{enumName}' with {rawNames.Count} entries at: {path}");
        }
        
        public static string GenerateEnumCode(string enumName, List<string> rawNames, List<int> values = null, string spacename = "Dark.Tools")
        {
            var used = new HashSet<string>();
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated. Do not edit by hand.");
            sb.AppendLine($"namespace {spacename}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");

            for (var i = 0; i < rawNames.Count; i++)
            {
                var raw = rawNames[i];
                var ident = ToValidIdentifier(raw);
                // ensure unique
                var final = ident;
                int suffix = 1;
                while (used.Contains(final))
                    final = ident + "_" + (++suffix).ToString();

                var value = values == null ? "" : $" = {values[i]}";
                used.Add(final);
                sb.AppendLine($"        {final}{value}, // \"{raw}\"");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
            
        private static string ToValidIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s)) return "_Empty";
            // Replace non-alphanumeric with underscores
            string id = Regex.Replace(s, @"[^a-zA-Z0-9_]", "_");
            // If starts with digit, prefix underscore
            if (Regex.IsMatch(id, @"^[0-9]")) id = "_" + id;
            // Collapse multiple underscores
            id = Regex.Replace(id, @"_+", "_");
            // Trim underscores from ends
            id = id.Trim('_');
            return string.IsNullOrEmpty(id) ? "_Unnamed" : id;
        }
    }
#endif
}
