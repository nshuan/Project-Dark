using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISelectSaveSlotInfo : MonoBehaviour
    {
        [SerializeField] private UISelectSaveSlotButton btnSelect;
        [SerializeField] private GameObject panelEmptySlot;
        [SerializeField] private GameObject panelSlot;
        [SerializeField] private TextMeshProUGUI txtClassName;
        [SerializeField] private TextMeshProUGUI txtDayPassed;
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private TextMeshProUGUI txtPlayedTime;
        [SerializeField] private Button btnClearSave;

        private SaveSlotManager saveSlotManager;
        
        private void Start()
        {
            saveSlotManager = SaveSlotManager.Instance;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var slotIndex = btnSelect.slotIndex;
            var isEmptySlot = saveSlotManager.IsEmptySlot(slotIndex);
            
            panelEmptySlot.SetActive(isEmptySlot);
            panelSlot.SetActive(!isEmptySlot);

            if (!isEmptySlot)
            {
                txtClassName.SetText(saveSlotManager.GetDisplayClassName(slotIndex));
                txtDayPassed.SetText(saveSlotManager.GetDisplayPassedDays(slotIndex));
                txtLevel.SetText(saveSlotManager.GetDisplayLevel(slotIndex));
                txtPlayedTime.SetText(saveSlotManager.GetDisplayTimePlayed(slotIndex));
            }
            
            btnClearSave.onClick.RemoveAllListeners();
            btnClearSave.onClick.AddListener(() =>
            {
                saveSlotManager.popupConfirmClearSave.gameObject.SetActive(true);
                saveSlotManager.popupConfirmClearSave.Setup(
                    "Clear save", 
                    $"Are you sure to clear data in Slot {btnSelect.Index}?",
                    () =>
                    {
                        saveSlotManager.ClearSlot(btnSelect.Index);
                        saveSlotManager.popupConfirmClearSave.gameObject.SetActive(false);
                        UpdateUI();
                    });
            });
        }
    }
}