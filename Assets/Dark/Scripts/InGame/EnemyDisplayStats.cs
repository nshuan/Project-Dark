using TMPro;
using UnityEngine;

namespace InGame
{
    public class EnemyDisplayStats : MonoBehaviour
    {
        [SerializeField] private TextMeshPro txtDmg;
        [SerializeField] private TextMeshPro txtSpeed;
        
        public void UpdateStats(int damage, float speed)
        {
            txtDmg.SetText(damage.ToString());
            txtSpeed.SetText(speed.ToString(GameConst.FloatFormat1));
        }
    }
}