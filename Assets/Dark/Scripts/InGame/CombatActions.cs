using System;

namespace InGame
{
    public class CombatActions
    {
        public static Action<float> OnAttackNormal { get; set; }
        public static Action<float> OnAttackCharge { get; set; }
        public static Action<float> OnMoveTower { get; set; }
        public static Action<float> OnTowerCounter { get; set; }
        public static Action<PassiveTriggerType, PassiveType, float> OnEffectTriggered { get; set; }
        public static Action<EnemyEntity> OnOneEnemyDead { get; set; }

        public static void Clear()
        {
            OnAttackNormal = null;
            OnAttackCharge = null;
            OnMoveTower = null;
            OnTowerCounter = null;
            OnEffectTriggered = null;
            OnOneEnemyDead = null;
        }
    }
}