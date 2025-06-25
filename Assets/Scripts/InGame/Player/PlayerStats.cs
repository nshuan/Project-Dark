using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Stats", fileName = "PlayerStats")]   
    public class PlayerStats : ScriptableObject
    {
        public int hp; // Each tower will take this hp value
        public int damage; // Player base damage
        public float cooldown; // %
        public float criticalRate; // Critical rate
        public float criticalDamage; // Critical Damage multiplier
    }
}