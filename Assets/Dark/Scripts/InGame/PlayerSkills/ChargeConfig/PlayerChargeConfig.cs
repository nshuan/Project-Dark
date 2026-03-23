using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.ChargeConfig
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Charge Config", fileName = "PlayerChargeConfig")]
    public class PlayerChargeConfig : SerializedScriptableObject
    {
        public int id;
        public float value;
        public float range; // hệ số scale của nhát chém charge size của knight charge size
        public float rangeStepMin; // if rangeStep after bonus is less than this value, override by this value
    }

    public enum ChargeType
    {
        Bullet,
        Size
    }
}