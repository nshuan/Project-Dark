using System.IO;
using Dark.Tools.Language.Runtime;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.Language.Editor
{
    public class GenerateLanguageConfig
    {
        [MenuItem("Tools/Dark/Language/Generate Data")]
        public static void CreateDataInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<LanguageData>(LanguageData.Path);
        }
        
        [MenuItem("Tools/Dark/Language/Generate Config")]
        public static void CreateConfigInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<LanguageConfig>(LanguageConfig.Path);
        }
    }
}