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
        public static Action OnChargeCooldownComplete { get; set; }
        public static Action<float> OnMoveTowerComplete { get; set; }
        public static Action OnMoveCooldownComplete { get; set; }
        public static Action<NodeTowerCounter.CounterType, float> OnTowerCounter { get; set; }
        public static Action<PassiveTriggerType, PassiveType, float> OnEffectTriggered { get; set; }
        public static Action<EnemyEntity, EnemyDieReason> OnOneEnemyDead { get; set; }
        public static Action<EnemyBehaviour, Vector2> OnBossKilled { get; set; } // Invoke with boss config and dead position
        public static Action<EnemyEntity> OnDropResource { get; set; }
        public static Action<EItemDropCollector> OnResourceCollectorInitialized { get; set; }
        public static Action<EItemDropCollector> OnResourceCollectorDamaged { get; set; }

        public static void Clear()
        {
            OnAttackNormal = null;
            OnAttackCharge = null;
            OnChargeCooldownComplete = null;
            OnMoveTowerComplete = null;
            OnMoveCooldownComplete = null;
            OnTowerCounter = null;
            OnEffectTriggered = null;
            OnOneEnemyDead = null;
            OnBossKilled = null;
            OnDropResource = null;
            OnResourceCollectorInitialized = null;
            OnResourceCollectorDamaged = null;
        }
    }
}