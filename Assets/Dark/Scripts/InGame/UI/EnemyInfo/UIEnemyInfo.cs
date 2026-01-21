using TMPro;
using UnityEngine;

namespace InGame.UI.EnemyInfo
{
    public class UIEnemyInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtEnemyName;
        [SerializeField] private TextMeshProUGUI txtDamage;
        [SerializeField] private TextMeshProUGUI txtHp;
        [SerializeField] private TextMeshProUGUI txtSpeed;
        [SerializeField] private TextMeshProUGUI txtAmount;

        public void UpdateUI(string enemyName, int hp, int atk, float speed, int amount)
        {
            txtEnemyName.SetText(enemyName);
            txtDamage.SetText($"Dmg: {atk}");
            txtHp.SetText($"Hp: {hp}");
            txtSpeed.SetText($"Spe: {speed}");
            txtAmount.SetText($"Amount: {amount}");
        }
    }
}