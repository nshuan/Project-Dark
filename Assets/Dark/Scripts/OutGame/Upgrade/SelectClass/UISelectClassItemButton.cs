using System;
using Dark.Scripts.OutGame.Common.NavButton;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UISelectClassItemButton : UIButton
    {
        [SerializeField] private RectTransform rectFrame;
        [SerializeField] private Image imgLight;
        [SerializeField] private Image imgIconLight;
        [SerializeField] private RectTransform rectSelected;

        [Space] [Header("Config")] [SerializeField]
        private UISelectClassItemConfig config;

        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    DoCollapse();
                    break;
                case UIButtonState.Hover:
                    break;
                case UIButtonState.Selected:
                    DoExpand();
                    break;
            }
        }

        private Tween DoExpand()
        {
            DOTween.Kill(this);
            return DOTween.Sequence(this)
                .Append(rectFrame
                    .DOSizeDelta(new Vector2(config.widthExpand, rectFrame.sizeDelta.y), config.expandDuration)
                    .SetEase(config.expandEasing))
                .Join(imgLight.DOFade(1f, config.lightOnDuration).SetEase(config.lightOnEasing))
                .Join(imgIconLight.DOFade(config.iconLightAlpha, config.lightOnDuration).SetEase(config.lightOnEasing))
                .AppendCallback(() => rectSelected.gameObject.SetActive(true));
        }

        private Tween DoCollapse()
        {
            DOTween.Kill(this);
            return DOTween.Sequence(this)
                .AppendCallback(() => rectSelected.gameObject.SetActive(false))
                .Append(rectFrame
                    .DOSizeDelta(new Vector2(config.widthCollapse, rectFrame.sizeDelta.y), config.expandDuration)
                    .SetEase(config.expandEasing))
                .Join(imgLight.DOFade(0f, config.lightOffDuration).SetEase(config.lightOffEasing))
                .Join(imgIconLight.DOFade(0f, config.lightOffDuration).SetEase(config.lightOffEasing));
        }

        [Button]
        public void SetCollapse()
        {
            rectFrame.sizeDelta = new Vector2(config.widthCollapse, rectFrame.sizeDelta.y);
        }

        [Button]
        public void SetExpand()
        {
            rectFrame.sizeDelta = new Vector2(config.widthExpand, rectFrame.sizeDelta.y);
        }
    }
}