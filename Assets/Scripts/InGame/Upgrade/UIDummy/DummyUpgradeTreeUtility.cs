using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade.UIDummy
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class DummyUpgradeTreeUtility
    {
        static DummyUpgradeTreeUtility()
        {
            var editor = GameObject.FindObjectOfType<DummyUpgradeTreeEditor>();
            if (editor == null)
            {
                DebugUtility.LogError("There is no Dummy Upgrade Tree Editor in the scene!!!");
                return;
            }

#if UNITY_EDITOR
            EditorApplication.hierarchyChanged += editor.EditorGetAllNodes;
#endif
        }
        
        
    }
}