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
        [OdinSerialize, NonSerialized] private Dictionary<Button, GameObject> tabDict;
        [SerializeField] private Button firstShowTab;
        
        private void Awake()
        {
            foreach (var tab in tabDict)
            {
                tab.Value.SetActive(false);
                tab.Key.onClick.RemoveAllListeners();
                tab.Key.onClick.AddListener(() =>
                {
                    foreach (var pair in tabDict)
                    {
                        pair.Value.SetActive(false);
                    }
                    
                    tab.Value.SetActive(true);
                });
            }
            
            firstShowTab.onClick.Invoke();
        }
    }
}