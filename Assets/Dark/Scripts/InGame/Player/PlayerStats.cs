using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Stats", fileName = "PlayerStats")]   
    public class PlayerStats : ScriptableObject
    {
        public int hp; // Each tower will take this hp value
        public int armor; // Base armor for each tower
        public int damageBase; // Player base damage
        public float damageRate; // Scale damage after add damageBase for each skill
        public int regen; // regenerate hp per second
        public float lifeLeech; // regenerate hp each time player deals damage
        public float cooldown; // %
        public float criticalRate; // Critical rate
        public float criticalDamage = 1f; // Critical Damage multiplier
        public float stagger; // Stagger base
        public float bossDamageScale = 1f; // Scale damage if dealing on boss
        public float vestigeCollectSize = 1f; 
    }
}