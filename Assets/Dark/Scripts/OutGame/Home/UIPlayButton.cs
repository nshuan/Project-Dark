using System.Collections;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIPlayButton : UIHomeButton
    {
        private void Start()
        {
            Loading.Instance.LoadSceneWithoutActivation(SceneConstants.SceneSaveSlot);
        }
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            Loading.Instance.LoadScene(SceneConstants.SceneSaveSlot);
            base.OnPointerClick(eventData);
        }
    }
}