using System;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class GateConfig
    {
        public Vector2 position;
        public int targetBaseIndex;
        
        public float startTime; // delay before the gate start spawning
        public float duration; // gate life time
        public EnemyType spawnType;
        public float intervalLoop = 4f; // duration between 2 spawns
        [NonSerialized, OdinSerialize] public IGateSpawner spawnLogic; // pattern for enemy appearance
    }
}