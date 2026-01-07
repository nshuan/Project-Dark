using UnityEngine;

namespace InGame.Shield
{
    [CreateAssetMenu(menuName = "InGame/Player/Tower Shield Config", fileName = "TowerShieldConfig")]
    public class TowerShieldConfig : ScriptableObject
    {
        public float healingInterval; // Thời gian liên tục ko dính dame thì bắt đầu hồi máu
        public int healingDuration; // Thời gian để hồi từ 0 lên max shield
        public float healingDelta; // Thời gian đợi hồi shield 1 lần
    }
}