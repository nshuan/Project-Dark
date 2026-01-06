using UnityEngine;

namespace InGame.Shield
{
    [CreateAssetMenu(menuName = "InGame/Player/Tower Shield Config", fileName = "TowerShieldConfig")]
    public class TowerShieldConfig : ScriptableObject
    {
        public float delayHealing;
        public int healingAmountPerTime;
        public float healingInterval;
    }
}