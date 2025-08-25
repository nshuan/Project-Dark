using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class ContinueButton : MonoBehaviour, IPointerClickHandler
    {
        private void Start()
        {
            Loading.Instance.LoadSceneWithoutActivation(SceneConstants.SceneUpgrade);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Loading.Instance.ActivateCacheScene();
        }
    }
}