using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIButtonOpenLink : MonoBehaviour, IPointerClickHandler
    {
        public string targetUrl;
        
        private bool interactable = true;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            interactable = false;
            this.DelayCall(1f, () => interactable = true);
            Application.OpenURL(targetUrl);    
        }
    }
}