using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings
{
    public class UISettings : MonoBehaviour
    {
        [SerializeField] private Button btnSave;
        [SerializeField] private List<UISettingItem> listSettingItems;

        private void Awake()
        {
            listSettingItems = GetComponentsInChildren<UISettingItem>().ToList();
            
            btnSave.onClick.RemoveAllListeners();
            btnSave.onClick.AddListener(() =>
            {
                if (listSettingItems is { Count: > 0 })
                {
                    btnSave.interactable = false;
                    this.DelayCall(1f, () => btnSave.interactable = true);
                    foreach (var item in listSettingItems)
                    {
                        item.Save();
                    }
                }
            });
        }
    }
}