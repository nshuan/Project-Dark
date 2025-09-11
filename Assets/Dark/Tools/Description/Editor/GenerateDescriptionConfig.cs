using System.IO;
using Dark.Tools.Description.Runtime;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.Description.Editor
{
    public class GenerateDescriptionConfig
    {
        [MenuItem("Tools/Dark/Description/Generate Data")]
        public static void CreateDataInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<DescriptionData>(DescriptionData.Path);
        }
        
        [MenuItem("Tools/Dark/Description/Generate Config")]
        public static void CreateConfigInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<DescriptionConfig>(DescriptionConfig.Path);
        }
    }
}