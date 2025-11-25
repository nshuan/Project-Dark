using System;
using Economic.InGame;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class CombatActions
    {
        public static Action<float> OnAttackNormal { get; set; }
        public static Action<float> OnAttackCharge { get; set; }
        public static Action<float> OnMoveTowerComplete { get; set; }
        public static Action<NodeTowerCounter.CounterType, float> OnTowerCounter { get; set; }
        public static Action<PassiveTriggerType, PassiveType, float> OnEffectTriggered { get; set; }
        public static Action<EnemyEntity> OnOneEnemyDead { get; set; }
        public static Action<EnemyBehaviour, Vector2> OnBossKilled { get; set; } // Invoke with boss config and dead position
        public static Action<EnemyEntity> OnCollectResource { get; set; }
        public static Action<EItemDropCollector> OnResourceCollectorDamaged { get; set; }

        public static void Clear()
        {
            OnAttackNormal = null;
            OnAttackCharge = null;
            OnMoveTowerComplete = null;
            OnTowerCounter = null;
            OnEffectTriggered = null;
            OnOneEnemyDead = null;
            OnBossKilled = null;
            OnCollectResource = null;
            OnResourceCollectorDamaged = null;
        }
    }
}