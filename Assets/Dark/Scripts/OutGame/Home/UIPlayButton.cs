using System.Collections;
using Dark.Scripts.Common;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIPlayButton : UIHomeButton
    {
        [SerializeField] private UISaveSlot panelSelectSaveSlot;
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            interactable = false;
            panelSelectSaveSlot.Open(UIConst.HomeBtnDelayOnClick);
            base.OnPointerClick(eventData);
        }
    }
}