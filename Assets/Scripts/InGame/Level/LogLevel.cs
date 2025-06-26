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
            LevelManager.Instance.OnLevelLoaded += OnLoadLevel;
            LevelManager.Instance.OnChangeTower += OnChangedTower;
            LevelManager.Instance.OnChangeSkill += OnChangedSkill;
        }

        private void OnLoadLevel(LevelConfig level)
        {
            txtLevel.SetText($"Level: {level.name}");
        }

        private void OnChangedTower(Transform tower)
        {
            currentTower.SetText($"Current tower: {tower.name} [{LevelManager.Instance.CurrentTower.CurrentHp}]");
        }

        private void OnChangedSkill(PlayerSkillConfig skillConfig)
        {
            txtSkill.SetText($"Skill: {skillConfig.name}");
            txtBulletDamage.SetText($"Base dmg per bullet: {LevelManager.Instance.PlayerStats.damage + skillConfig.damePerBullet}");
            txtSkillCooldown.SetText($"Skill cooldown: {skillConfig.cooldown}");
            txtAttackRange.SetText($"Attack range: {skillConfig.range}");
        }
    }
}