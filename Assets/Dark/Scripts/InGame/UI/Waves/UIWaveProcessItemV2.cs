using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.Waves
{
    public class UIWaveProcessItemV2 : MonoBehaviour
    {
        [SerializeField] private Image imgPassed;
        
        public void DoPassed()
        {
            imgPassed.SetAlpha(0f);
            imgPassed.gameObject.SetActive(true);
            imgPassed.DOFade(1f, 0.5f);
        }
    }
}