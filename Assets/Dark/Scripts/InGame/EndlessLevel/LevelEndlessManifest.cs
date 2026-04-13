using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;

namespace InGame.EndlessLevel
{
    public class LevelEndlessManifest : SerializedScriptableObject
    {
        private static string Path = "LevelEndlessManifest";
        private static string FilePath = "Assets/Dark/Resources/LevelEndlessManifest.asset";
        
#if UNITY_EDITOR
        
        [MenuItem("Dark/Manifest/Generate Level Endless Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<LevelManifest>(FilePath);
        }
        
        [Button]
        public void Validate()
        {

        }
        
#endif
    }
}