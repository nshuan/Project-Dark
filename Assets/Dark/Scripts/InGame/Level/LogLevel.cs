using System;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class LogLevel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtLevel;
        
        [Space] [Header("Player")]
        [SerializeField] private TextMeshProUGUI txtSkill;
        [SerializeField] private TextMeshProUGUI txtBulletDamage;
        [SerializeField] private TextMeshProUGUI txtSkillCooldown;
        [SerializeField] private TextMeshProUGUI txtAttackRange;
        
        [Space] [Header("Tower")]
        // [SerializeField] private TextMeshProUGUI towersHp;
        [SerializeField] private TextMeshProUGUI currentTower;

        [Space] [Header("Enemy & Gate")] 
        [SerializeField] private TextMeshProUGUI totalGate;
        // [SerializeField] private TextMeshProUGUI totalEnemy;

        private void Awake()
        {
            LevelManager.Instance.OnLevelPreLoaded += OnLevelPreLoaded;
            LevelManager.Instance.OnChangeTower += OnChangedTower;
        }

        private void OnLevelPreLoaded(LevelConfig level)
        {
            txtLevel.SetText($"Level: {level.name}");

            var skillConfig = LevelUtilityV2.StatsNormalAttack;
            txtSkill.SetText($"Skill: {skillConfig.name}");
            txtBulletDamage.SetText($"Base dmg per bullet: {LevelUtilityV2.GetNormalAttackDamage()}");
            txtSkillCooldown.SetText($"Skill cooldown: {skillConfig.cooldown}");
            txtAttackRange.SetText($"Attack range: {skillConfig.range}");
        }

        private void OnChangedTower(TowerEntity tower)
        {
            currentTower.SetText($"Current tower: {tower.name} [{LevelManager.Instance.CurrentTower.CurrentHp}]");
        }
    }
}