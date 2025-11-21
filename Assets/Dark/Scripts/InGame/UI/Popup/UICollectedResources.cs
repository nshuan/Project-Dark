using System;
using Economic.InGame.DropItems;
using TMPro;
using UnityEngine;

namespace InGame.UI
{
    public class UICollectedResources : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtVestige;
        [SerializeField] private TextMeshProUGUI txtSigils;
        [SerializeField] private TextMeshProUGUI txtEchoes;

        private void OnEnable()
        {
            txtVestige.SetText(EItemDropManager.Instance.CollectedData.TotalCollectedVestige.ToString());
            txtSigils.SetText(EItemDropManager.Instance.CollectedData.TotalCollectedSigils.ToString());
            txtEchoes.SetText(EItemDropManager.Instance.CollectedData.TotalCollectedEchoes.ToString());
        }
    }
}