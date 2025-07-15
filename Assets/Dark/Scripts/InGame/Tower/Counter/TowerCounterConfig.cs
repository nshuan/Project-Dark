using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Tower/Tower Counter Config", fileName = "TowerCounterConfig")]
    public class TowerCounterConfig : SerializedScriptableObject
    {
        public int id;
        [NonSerialized, OdinSerialize] public ITowerCounterLogic logic;
        public int damage;
        public float cooldown;
    }
}