using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.CounterConfig
{ 
    [CreateAssetMenu(menuName = "InGame/Player/Tower Counter Config", fileName = "TowerCounterConfig")]
    public class TowerCounterConfig : SerializedScriptableObject
    {
        public int id;
        public int damage;
        public float cooldown;
        public float stagger;
        public float range;
        public float size; // Amount của piercing
    }
}