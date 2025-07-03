using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace InGame.Upgrade.UIDummy
{
    public class DummyUpgradeTree : SerializedMonoBehaviour
    {
        [field: ReadOnly, NonSerialized, OdinSerialize] public DummyUpgradeNode[] Nodes { get; set; }
    }
}