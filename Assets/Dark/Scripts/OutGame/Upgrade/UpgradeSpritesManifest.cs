using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Sprites Manifest", fileName = "UpgradeSpritesManifest")]
    public class UpgradeSpritesManifest : SerializedScriptableObject
    {
        [SerializeField] private UpgradeTreeConfig tree;
        [SerializeField] private Dictionary<int, UpgradeNodeSpriteInfo> spriteMap;
        [SerializeField] private string folderPath= "Assets/Dark/Sprites/OutGame/Upgrade/Export000/NodeEffects/UI_Icon_effect_HeavensPierce.png";

        [Button]
        public void ValidateSprites()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(Sprite), new[] { folderPath });
            List<Sprite> assets = new List<Sprite>();
            foreach (string guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (asset != null)
                    assets.Add(asset);
            }
            
            spriteMap = new Dictionary<int, UpgradeNodeSpriteInfo>();
            foreach (var normalSprite in assets)
            {
                var nodeName = normalSprite.name.Substring(normalSprite.name.LastIndexOf('_') + 1);
                nodeName = nodeName.Replace(" ", "");
                var config = tree.GetNodeByName(nodeName);
                if (config == null) continue;

                var lockSprite = assets.FirstOrDefault((asset) =>
                    asset.name.Replace(" ", "") == nodeName && asset.name.Contains("locked"));
                lockSprite = lockSprite ? lockSprite : normalSprite;
                spriteMap[config.nodeId] = new UpgradeNodeSpriteInfo()
                {
                    normalSprite = normalSprite,
                    lockSprite = lockSprite,
                };
            }
            
            // Mark asset dirty so Unity knows to save it
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public UpgradeNodeSpriteInfo GetSprite(int id)
        {
            if (spriteMap.ContainsKey(id))
                return spriteMap[id];

            return null;
        }
    }

    [Serializable]
    public class UpgradeNodeSpriteInfo
    {
        public Sprite normalSprite;
        public Sprite lockSprite;
    }
}