using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class ContinueButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
        }
    }
}