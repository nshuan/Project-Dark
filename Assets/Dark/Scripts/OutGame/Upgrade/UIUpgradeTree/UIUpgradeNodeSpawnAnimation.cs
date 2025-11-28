using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeSpawnAnimation : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] public IUpgradeNodeSpawnLogic spawnLogic;

        public IUpgradeNodeSpawnLogic SpawnLogic => spawnLogic;
    }
}