using System;
using System.Collections.Generic;
using InGame.Upgrade.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade.UIDummy
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Upgrade Tree", fileName = "UpgradeTreeConfig")]
    public class DummyUpgradeTreeConfig : SerializedScriptableObject
    {
        [field: ReadOnly, NonSerialized, OdinSerialize]
        public Dictionary<int, UpgradeNodeConfig> nodeMapById;
        
        [Space]
        [ReadOnly]
        public UIUpgradeTree treePrefab;
    }
}