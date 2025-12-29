using UnityEngine;

namespace InGame.Shield
{
    [CreateAssetMenu(menuName = "InGame/Player/Tower Shield Config", fileName = "TowerShieldConfig")]
    public class TowerShieldConfig : ScriptableObject
    {
        public int maxShield;
        public float delayHealing;
        public int healingAmountPerTime;
        public float healingInterval;
    }
}