using Core;
using DG.Tweening;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScrollView : MonoBehaviour
    {
        [SerializeField] private ZoomInOut zoom;
        [SerializeField] private RectTransform content;

        public void FocusTo(RectTransform target)
        {
           zoom.ZoomTo(0.8f, target.position, 0.3f, 0f);
        }
    }
}