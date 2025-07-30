using System;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class GateConfig
    {
        public bool isBossGate;
        public Vector2 position;
        public bool showVisual = true;
        [Range(0, 2)] public int[] targetBaseIndex;
        
        public float startTime; // delay before the gate start spawning
        public float duration; // gate life time
        public EnemyBehaviour spawnType;
        public float intervalLoop = 4f; // duration between 2 spawns
        [NonSerialized, OdinSerialize] public IGateSpawner spawnLogic; // pattern for enemy appearance
    }
}