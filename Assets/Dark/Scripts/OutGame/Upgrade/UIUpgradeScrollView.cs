using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScrollView : MonoBehaviour
    {
        [SerializeField] private ZoomInOut zoom;
        [SerializeField] private RectTransform content;
        [SerializeField] private float focusDuration = 0.5f;

        public void FocusTo(RectTransform target)
        {
           zoom.ZoomTo(0.8f, target.position, focusDuration, 0f);
        }
    }
}