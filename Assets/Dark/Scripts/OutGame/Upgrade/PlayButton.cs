using Dark.Scripts.SceneNavigation;
using InGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
            {
                LevelManager.Instance.LoadLevel(1);
            });
        }
    }
}