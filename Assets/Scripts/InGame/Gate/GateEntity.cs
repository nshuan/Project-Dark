using System;
using UnityEngine;

namespace InGame
{
    public class GateEntity : MonoBehaviour
    {

    }

    [Serializable]
    public class GateData
    {
        public float startTime; // Time to wait before start spawning
        public float duration; // Gate lifetime
        public GameObject enemy; // prefab of the enemy that spawn from this gate
        public float intervalLoop; // spawn cooldown
        public int groupPattern; // pattern for enemy appearing phase
    }
}