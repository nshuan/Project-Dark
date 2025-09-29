using System.Collections;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIPlayButton : UIHomeButton
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            interactable = false;
            Loading.Instance.LoadScene(SceneConstants.SceneSaveSlot);
            base.OnPointerClick(eventData);
        }
    }
}