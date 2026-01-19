using TMPro;
using UnityEngine;

namespace InGame.UI.EnemyInfo
{
    public class UIEnemyInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtEnemyName;
        [SerializeField] private TextMeshProUGUI txtDamage;
        [SerializeField] private TextMeshProUGUI txtHp;
        
        public int EnemyId { get; set; }
        
        public void UpdateUI(EnemyEntity enemy)
        {
            if (!enemy || !enemy.config) return;
            EnemyId = enemy.config.enemyId;
            
            txtEnemyName.SetText(enemy.config.displayName);
            txtDamage.SetText($"Dmg: {enemy.CurrentDamage}");
            txtHp.SetText($"Hp: {enemy.MaxHealth}");
        }
    }
}