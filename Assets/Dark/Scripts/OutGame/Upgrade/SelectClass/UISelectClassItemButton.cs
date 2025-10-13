using System;
using Dark.Scripts.OutGame.Common.NavButton;
using Data;
using DG.Tweening;
using InGame.CharacterClass;
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
        [SerializeField] private Button btnSelect;
        [SerializeField] private Image imgSelected;

        [Space] [Header("Config")]
        [SerializeField] private CharacterClass classType;
        [SerializeField] private UISelectClassItemConfig config;

        public override void UpdateUI(UIButtonState state)
        {
            btnSelect.gameObject.SetActive(true);
            imgSelected.gameObject.SetActive(false);
            
            switch (state)
            {
                case UIButtonState.None:
                    rectSelected.gameObject.SetActive(false);
                    btnSelect.onClick.RemoveAllListeners();
                    DoCollapse();
                    break;
                case UIButtonState.Hover:
                    rectSelected.gameObject.SetActive(true);
                    break;
                case UIButtonState.Selected:
                    btnSelect.onClick.RemoveAllListeners();
                    btnSelect.onClick.AddListener(() =>
                    {
                        // btnSelect.gameObject.SetActive(false);
                        // imgSelected.gameObject.SetActive(true);
                        
                        // Select class
                        UIUpgradeScene.Instance.SelectClass(classType);
                    });
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
                .Join(imgIconLight.DOFade(config.iconLightAlpha, config.lightOnDuration).SetEase(config.lightOnEasing));
        }

        private Tween DoCollapse()
        {
            DOTween.Kill(this);
            return DOTween.Sequence(this)
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