using System;
using Dark.Scripts.Common;
using Dark.Scripts.Utils;
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
        [SerializeField] private GameObject[] imgClass;
        [SerializeField] private GameObject[] imgClassLight;
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

            foreach (var icon in imgClass)
                icon.SetActive(false);
            foreach (var icon in imgClassLight)
                icon.SetActive(false);
            
            btnClearSave.onClick.RemoveAllListeners();
            
            if (!isEmptySlot)
            {
                var classType = saveSlotManager.GetClassTypeIndex(slotIndex);
                if (classType >= 0 || classType < imgClass.Length) imgClass[classType].SetActive(true);
                if (classType >= 0 || classType < imgClassLight.Length) imgClassLight[classType].SetActive(true);
                
                txtClassName.SetText(saveSlotManager.GetDisplayClassName(slotIndex));
                txtDayPassed.SetText(saveSlotManager.GetDisplayPassedDays(slotIndex));
                txtLevel.SetText(saveSlotManager.GetDisplayLevel(slotIndex));
                txtPlayedTime.SetText(saveSlotManager.GetDisplayTimePlayed(slotIndex));
                
                btnClearSave.onClick.AddListener(() =>
                { 
                    saveSlotManager.popupConfirmClearSave.Setup(
                        "your data will be lost", 
                        $"Are you sure?",
                        () =>
                        {
                            saveSlotManager.ClearSlot(btnSelect.Index);
                            saveSlotManager.popupConfirmClearSave.gameObject.SetActive(false);
                            UpdateUI();
                        });
                    this.DelayCall(UIConst.BtnDelayOnClick, () =>
                    {
                        saveSlotManager.popupConfirmClearSave.gameObject.SetActive(true);
                    });
                });
            }
        }
    }
}