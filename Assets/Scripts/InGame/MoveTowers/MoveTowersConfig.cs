using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Move Towers Config", fileName = "MoveTowersConfig")]
    public class MoveTowersConfig : SerializedScriptableObject
    {
        public int id;
        public float delayMove;
        public float cooldown;
        public int damage;
        public float size;
    }
}