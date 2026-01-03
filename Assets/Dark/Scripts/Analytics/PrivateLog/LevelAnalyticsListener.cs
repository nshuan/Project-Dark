using System;
using Economic;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Analytics.PrivateLog
{
    public class LevelAnalyticsListener : MonoBehaviour
    {
        private int level;
        private int wave;
        private int vestigeDropped;
        private int vestigeCollected;
        private int expCollected;
        private int dmgDealed;
        private int dmgReceived;

        private int lastVestige;
        private int lastExp;
        private int lastLoggedWave = -1;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWaveStart += OnWaveStart;
            LevelManager.Instance.onWaveEnded += OnWaveEnded;
            CombatActions.OnDamageDealt += OnDamageDealt;
            CombatActions.OnDamageReceived += OnDamageReceived;
            CombatActions.OnDropResource += OnDropResource;
            
            LevelAnalyticsLogger.Initialize();
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            this.level = level.level;
        }

        private void OnDamageDealt(int value)
        {
            dmgDealed += value;
        }

        private void OnDamageReceived(int value)
        {
            dmgReceived += value;
        }
        
        private void OnDropResource(EnemyEntity enemy, bool isDropVestige)
        {
            if (isDropVestige)
            {
                vestigeDropped += enemy.Dark * enemy.DarkUnitValue;
            }
        }

        private void OnWaveStart(int wave, float duration)
        {
            this.wave = wave;
            lastVestige = WealthManager.Instance.Vestige;
            lastExp = WealthManager.Instance.Exp;
            dmgDealed = 0;
            dmgReceived = 0;
            vestigeDropped = 0;
        }

        private void OnWaveEnded(int wave, WaveEndReason reason)
        {
            if (wave <= lastLoggedWave) return;
            vestigeCollected = WealthManager.Instance.Vestige - lastVestige;
            expCollected = WealthManager.Instance.Exp - lastExp;

            lastLoggedWave = this.wave;
            LevelAnalyticsLogger.Log(level, this.wave, vestigeDropped, vestigeCollected, expCollected, dmgDealed, dmgReceived, reason.ToString());
        }
    }
}