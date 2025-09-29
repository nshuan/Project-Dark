using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    public class TestUpgradeBonus : SerializedMonoBehaviour
    {
        [SerializeField] private bool enableTest;
        
        [NonSerialized, OdinSerialize] public UpgradeBonusInfo testBonusInfo;

        private void Awake()
        {
#if UNITY_EDITOR
            if (enableTest)
                UpgradeManager.Instance.ForceTestBonusInfo(testBonusInfo);
#endif
        }

        [Button]
        public void Reset()
        {
            testBonusInfo = new UpgradeBonusInfo();
        }
        
#if UNITY_EDITOR
        [Button]
        public void ForceReactivate()
        {
            UpgradeManager.Instance.ForceTestBonusInfo(testBonusInfo);
            UpgradeManager.Instance.ForceReactivateTree();
        }
#endif
    }
}