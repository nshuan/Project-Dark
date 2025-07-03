using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace InGame.Upgrade.UI
{
    public class UIUpgradeTree : SerializedMonoBehaviour
    {
        [field: ReadOnly, NonSerialized, OdinSerialize] public UIUpgradeNode[] Nodes { get; set; }
    }
}