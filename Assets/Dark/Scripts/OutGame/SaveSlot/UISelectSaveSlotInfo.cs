using System;
using Coffee.UIExtensions;
using Dark.Scripts.Common;
using Dark.Scripts.Utils;
using Dark.Tools.Language.Runtime;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISelectSaveSlotInfo : MonoBehaviour
    {
        [SerializeField] private UISelectSaveSlotButton btnSelect;
        public CanvasGroup canvasGroup;
        [SerializeField] private GameObject panelEmptySlot;
        [SerializeField] private GameObject panelSlot;
        [SerializeField] private GameObject[] imgClass;
        [SerializeField] private GameObject[] imgClassLight;
        [SerializeField] private TextMeshProUGUI txtClassName;
        [SerializeField] private TextMeshProUGUI txtDayPassed;
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private TextMeshProUGUI txtPlayedTime;
        [SerializeField] private Button btnClearSave;

        public Action<int> ActionClearSaveSlot { get; set; }
        
        public void UpdateUI()
        {
            var slotIndex = btnSelect.slotIndex;
            var isEmptySlot = SaveSlotManager.Instance.IsEmptySlot(slotIndex);
            
            panelEmptySlot.SetActive(isEmptySlot);
            panelSlot.SetActive(!isEmptySlot);

            foreach (var icon in imgClass)
                icon.SetActive(false);
            foreach (var icon in imgClassLight)
                icon.SetActive(false);
            
            btnClearSave.gameObject.SetActive(!isEmptySlot);
            btnClearSave.onClick.RemoveAllListeners();
            
            if (!isEmptySlot)
            {
                var classType = SaveSlotManager.Instance.GetClassTypeIndex(slotIndex);
                if (classType >= 0 && classType < imgClass.Length) imgClass[classType].SetActive(true);
                if (classType >= 0 && classType < imgClassLight.Length) imgClassLight[classType].SetActive(true);
                
                txtClassName.SetText(SaveSlotManager.Instance.GetDisplayClassName(slotIndex));
                txtDayPassed.SetText(SaveSlotManager.Instance.GetDisplayPassedDays(slotIndex));
                // if (SaveSlotManager.Instance.IsSlotCompleted(slotIndex))
                // {
                //     txtLevel.SetTextLanguage("key_save_slot_level",
                //         ("%{value}",
                //             LanguageData.Instance.GetLocalizedString("key_complete",
                //                 LanguageManager.Instance.CurrentLanguage)));
                // }
                // else
                { 
                    txtLevel.SetText(SaveSlotManager.Instance.GetDisplayLevel(slotIndex));
                }
                txtPlayedTime.SetText(SaveSlotManager.Instance.GetDisplayTimePlayed(slotIndex));
                
                btnClearSave.onClick.AddListener(() =>
                { 
                    ActionClearSaveSlot?.Invoke(slotIndex);
                });
            }
        }
    }
}