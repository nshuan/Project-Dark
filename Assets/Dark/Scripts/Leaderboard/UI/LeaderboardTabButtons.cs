using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public class LeaderboardTabButtons : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private Dictionary<Button, LeaderboardButtonInfo> tabDict;
        [SerializeField] private Button firstShowTab;
        
        private void Awake()
        {
            foreach (var tab in tabDict)
            {
                tab.Value.viewTab.SetActive(false);
                tab.Value.btnLight.SetActive(false);
                tab.Key.onClick.RemoveAllListeners();
                tab.Key.onClick.AddListener(() =>
                {
                    foreach (var pair in tabDict)
                    {
                        pair.Value.viewTab.SetActive(false);
                        pair.Value.btnLight.SetActive(false);
                    }
                    
                    tab.Value.viewTab.SetActive(true);
                    tab.Value.btnLight.SetActive(true);
                });
            }
            
            firstShowTab.onClick.Invoke();
        }
        
        [Serializable]
        public class LeaderboardButtonInfo
        {
            public GameObject viewTab;
            public GameObject btnLight;
        }
    }
}