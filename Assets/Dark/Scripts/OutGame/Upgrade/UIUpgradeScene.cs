using System;
using Core;
using Dark.Scripts.Analytics;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.OutGame.Intro;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using InGame;
using InGame.CharacterClass;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScene : MonoSingleton<UIUpgradeScene>
    {
        [Space] [Header("Upgrade Tree")]
        [SerializeField] private UIPanelUpgradeTree panelUpgradeTree;

        [Space] [Header("Select class")] 
        [SerializeField] private GameObject panelSelectClass;

        [SerializeField] private Color loadingIntroColor;

        [Space] [Header("Confirm")] 
        [SerializeField] private UIPopupConfirmExchange popupConfirmExchange;
        [SerializeField] private UIPopupConfirmReset popupConfirmReset;
        
        public UIPopupConfirmExchange PopupConfirmExchange => popupConfirmExchange;
        public UIPopupConfirmReset PopupConfirmReset => popupConfirmReset;
        
        protected override void Awake()
        {
            if (PlayerDataManager.Instance.Data.initialized == false)
            {
                panelUpgradeTree.gameObject.SetActive(false);
                panelSelectClass.SetActive(true);
            }
            else
            {
                panelUpgradeTree.gameObject.SetActive(true);
                panelSelectClass.SetActive(false);
                panelUpgradeTree.SpawnTree();
            }
        }

        public void SelectClass(CharacterClass classType)
        {
            // Save selected class
            if (PlayerDataManager.Instance.Data.initialized == false)
            {
                var data = PlayerDataManager.Instance.Data;
                data.characterClass = (int)classType;
                data.initialized = true;
                
                PlayerDataManager.Instance.Save(data);
                
                var totalDataCreated = SaveSlotManager.Instance.GetTotalDataCreated();
                totalDataCreated += 1;
                SaveSlotManager.Instance.SaveTotalDataCreated(totalDataCreated);
                LogManager.Log(LogConst.EventLogTotalDataSlotsCreated, totalDataCreated.ToString());
            }
            
#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            AudioManagerV2.Instance.StopMusic(1.5f);
            
            this.DelayCall(0.5f, () =>
            {
                LogManager.Log(LogConst.EventLogStartLevel, $"level_{PlayerDataManager.Instance.Data.level + 1}", "from class select");

                UIIntroScene.OnCompleteIntro += () =>
                {
                    Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, overrideHideDuration: 1f);
                    Loading.Instance.onSceneLoaded += () =>
                    {
                        LevelManager.Instance.LoadLevel(1);
                    };
                };
                
                Loading.Instance.OverrideQuickLoadBgColorOnce(loadingIntroColor);
                Loading.Instance.QuickLoadScene(SceneConstants.SceneIntro, overrideOpenDuration: 1f);
            });
        }
    }
}