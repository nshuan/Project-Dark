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
        public static void GenerateEnumScript(string path, List<string> rawNames, string enumName,
            string spacename = "Dark.Tools")
        {
            var code = GenerateEnumCode(rawNames, enumName, spacename);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, code, new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.Log($"Generated enum '{enumName}' with {rawNames.Count} entries at: {path}");
        }
        
        public static string GenerateEnumCode(List<string> rawNames, string enumName, string spacename = "Dark.Tools")
        {
            var used = new HashSet<string>();
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated. Do not edit by hand.");
            sb.AppendLine($"namespace {spacename}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");

            foreach (var raw in rawNames)
            {
                var ident = ToValidIdentifier(raw);
                // ensure unique
                var final = ident;
                int suffix = 1;
                while (used.Contains(final))
                    final = ident + "_" + (++suffix).ToString();

                used.Add(final);
                sb.AppendLine($"        {final}, // \"{raw}\"");
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
