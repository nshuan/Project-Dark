using System;
using Economic.InGame;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class CombatActions
    {
        public static Action<GateEntity, int, int> OnGateActivated { get; set; } // <WaveIndex, GateIndex>
        public static Action<float> OnAttackNormal { get; set; }
        public static Action<float> OnAttackCharge { get; set; }
        public static Action OnChargeCooldownComplete { get; set; }
        public static Action<TowerEntity> OnTowerHoverIn { get; set; }
        public static Action<TowerEntity> OnTowerHoverOut { get; set; }
        public static Action<float> OnMoveTowerComplete { get; set; }
        public static Action OnMoveCooldownComplete { get; set; }
        public static Action<NodeTowerCounter.CounterType, float> OnTowerCounter { get; set; }
        public static Action<PassiveTriggerType, PassiveType, float> OnEffectTriggered { get; set; }
        public static Action<EnemyEntity, EnemyDieReason> OnOneEnemyDead { get; set; }
        public static Action<EnemyEntity> OnOneEnemySpawn { get; set; }
        public static Action<EnemyEntity, Vector2> OnBossKilled { get; set; } // Invoke with boss config and dead position
        public static Action<EnemyEntity, bool> OnDropResource { get; set; } // <Dropper, hasVestige>
        public static Action<EItemDropCollector> OnResourceCollectorInitialized { get; set; }
        public static Action<EItemDropCollector> OnResourceCollectorDamaged { get; set; }
        public static Action<bool, int, float> OnCollectAllResourceDrop { get; set; } // <isAutoCollect, amount, duration>
        public static Action<int> OnDamageDealt { get; set; }
        public static Action<int> OnDamageReceived { get; set; }
        public static Action<MonoCursor> OnInitInGameCursor { get; set; }
        public static Action OnChargeStarted { get; set; }
        public static Action OnChargeEnded { get; set; }

        public static void Clear()
        {
            OnGateActivated = null;
            OnAttackNormal = null;
            OnAttackCharge = null;
            OnChargeCooldownComplete = null;
            OnTowerHoverIn = null;
            OnTowerHoverOut = null;
            OnMoveTowerComplete = null;
            OnMoveCooldownComplete = null;
            OnTowerCounter = null;
            OnEffectTriggered = null;
            OnOneEnemyDead = null;
            OnOneEnemySpawn = null;
            OnBossKilled = null;
            OnDropResource = null;
            OnResourceCollectorInitialized = null;
            OnResourceCollectorDamaged = null;
            OnDamageDealt = null;
            OnDamageReceived = null;
            OnInitInGameCursor = null;
            OnChargeStarted = null;
            OnChargeEnded = null;
        }
    }
}