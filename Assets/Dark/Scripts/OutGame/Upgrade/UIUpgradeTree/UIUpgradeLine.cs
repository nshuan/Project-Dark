using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeLine : MonoBehaviour
    {
        [SerializeField] private GameObject lineAvailable;
        [SerializeField] private GameObject lineLocked;
        [SerializeField] private GameObject lineActivated;

        public void UpdateLineState(UIUpgradeNodeState state)
        {
            lineAvailable.SetActive(state == UIUpgradeNodeState.Available);
            lineLocked.SetActive(state == UIUpgradeNodeState.Locked);
            lineActivated.SetActive(state == UIUpgradeNodeState.Activated);
        }
    }
}