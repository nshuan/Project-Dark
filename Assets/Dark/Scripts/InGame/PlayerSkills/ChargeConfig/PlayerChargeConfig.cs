using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.ChargeConfig
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Charge Config", fileName = "PlayerChargeConfig")]
    public class PlayerChargeConfig : SerializedScriptableObject
    {
        public int id;
        public float value;
        public float range;
    }

    public enum ChargeType
    {
        Bullet,
        Size
    }
}