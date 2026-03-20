using Sirenix.OdinInspector;
using UnityEngine;

namespace Dark.Scripts.InGame.Upgrade
{
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Reset Config", fileName = "UpgradeResetConfig")]
    public class UpgradeResetConfig : SerializedScriptableObject
    {
        public float refundVestigeRatio = 1f;
        public float refundEchoesRatio = 1f;
        public float refundSigilsRatio = 1f;
    }
}