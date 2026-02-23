using System.Collections.Generic;
using UnityEngine;

namespace InGame.BossConfig
{
    [CreateAssetMenu(menuName = "InGame/Boss/Boss Wizard", fileName = "BossWizard")]
    public class BossWizardConfig : BossBehaviourConfig
    {
        public bool summonOnSpawn;
        public bool summonContinuously;
        public float summonInterval;
        public float phase2HpPercentage;
        public int phase2TowerId;
        public List<int> listSummonIdsOnPhase2;
        public List<int> listSummonAmountOnPhase2;
        public float phase2ScaleSpeed = 1f;
        public float phase2ScaleDamage = 1f;
        public float phase2DelayTakeDamage = 1f;
    }
}