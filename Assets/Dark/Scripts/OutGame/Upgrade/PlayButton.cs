using Dark.Scripts.Common;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using InGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler
    {
        private bool interactable = true;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;
            interactable = false;

#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            this.DelayCall(0.5f, () =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }
    }
}