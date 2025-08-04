using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeLine : MonoBehaviour
    {
        [SerializeField] private GameObject lineAvailable;
        [SerializeField] private GameObject lineLocked;
        [SerializeField] private GameObject lineActivated;
        [SerializeField] private GameObject lineGlow;

        public void UpdateLineState(UIUpgradeNodeState state)
        {
            lineAvailable.SetActive(state == UIUpgradeNodeState.Available);
            lineLocked.SetActive(state == UIUpgradeNodeState.Locked);
            lineActivated.SetActive(state == UIUpgradeNodeState.Activated);
            lineGlow.SetActive(state == UIUpgradeNodeState.Activated);
        }
    }
}