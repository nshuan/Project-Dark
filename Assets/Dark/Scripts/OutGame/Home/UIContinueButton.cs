using Dark.Scripts.SceneNavigation;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIContinueButton : UIHomeButton
    {
        private void Start()
        {
            Loading.Instance.LoadSceneWithoutActivation(SceneConstants.SceneUpgrade);
        }
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            Loading.Instance.ActivateCacheScene();
            base.OnPointerClick(eventData);
        }
    }
}