using System;
using Dark.Scripts.Analytics;
using Dark.Scripts.Common;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using InGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image imgEndlessDecor;
        
        private bool interactable = true;

        private void OnEnable()
        {
            var levelToLoad = PlayerDataManager.Instance.Data.level + 1;
            var maxLevel = LevelManifest.Instance.GetMaxLevel(PlayerDataManager.Instance.Data.Class);
            imgEndlessDecor.gameObject.SetActive(levelToLoad > maxLevel);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;
            interactable = false;

#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            this.DelayCall(0.5f, () =>
            {
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame);
                Loading.Instance.onSceneLoaded += () =>
                {
                    var levelToLoad = PlayerDataManager.Instance.Data.level + 1;
                    var maxLevel = LevelManifest.Instance.GetMaxLevel(PlayerDataManager.Instance.Data.Class);
                    if (levelToLoad > maxLevel)
                    {
                        levelToLoad = maxLevel;
                        LevelManager.Instance.LoadEndlessLevel();
                    }
                    else
                    {
                        LevelManager.Instance.LoadLevel(levelToLoad);
                    }
                };
            });
            
            LogManager.Log(LogConst.EventLogStartLevel, $"level_{PlayerDataManager.Instance.Data.level + 1}", "from upgrade");
        }
    }
}