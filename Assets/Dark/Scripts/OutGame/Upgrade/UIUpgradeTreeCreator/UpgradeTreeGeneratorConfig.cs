using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    [CreateAssetMenu(menuName = "Tool/Upgrade Tree Generator Config", fileName = "UpgradeTreeGeneratorConfig")]
    public class UpgradeTreeGeneratorConfig : SerializedScriptableObject
    {
        [SerializeField] public Canvas canvasPrefab;
        [SerializeField] public UIUpgradeTree treePrefab;
        [OdinSerialize, NonSerialized] public Dictionary<NodeType, List<GameObject>> nodePrefabsMap;
    }
}