using Core;
using UnityEngine;

namespace InGame.UI
{
    public class CanvasInGame : MonoSingleton<CanvasInGame>
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public void ShowUI()
        {
            canvasGroup.alpha = 1;
        }
        
        public void HideUI()
        {
            canvasGroup.alpha = 0;
        }
    }
}