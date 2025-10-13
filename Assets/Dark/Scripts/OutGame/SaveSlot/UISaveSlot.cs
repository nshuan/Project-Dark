using System;
using Dark.Scripts.Common;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISaveSlot : MonoBehaviour
    {
        [SerializeField] private UISelectSaveSlotInfo[] saveSlots;
        public UIPopupWarning popupConfirmClearSave;
        [SerializeField] private Button btnBack;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(() =>
            {
                btnBack.interactable = false;
                DoClose(UIConst.BtnDelayOnClick);
            });
        }

        public void Open(float delay)
        {
            foreach (var slot in saveSlots)
            {
                slot.ActionClearSaveSlot = (int slotIndex) =>
                {
                    popupConfirmClearSave.Setup(
                        "your data will be lost",
                        $"Are you sure?",
                        () =>
                        {
                            SaveSlotManager.Instance.ClearSlot(slotIndex);
                            popupConfirmClearSave.DoCloseFadeOut();
                            slot.UpdateUI();
                        });
                    
                    popupConfirmClearSave.DoOpenFadeIn(UIConst.BtnDelayOnClick);
                };
                
                slot.UpdateUI();
            }

            DoOpen(delay);
        }

        public Tween DoOpen(float delayOpen)
        {
            DOTween.Kill(this);
            canvasGroup.alpha = 0f;
            if (!btnBack.TryGetComponent<CanvasGroup>(out var btnBackGraphic))
                btnBackGraphic = btnBack.AddComponent<CanvasGroup>();
            btnBackGraphic.alpha = 0f;
            gameObject.SetActive(true);
            return DOTween.Sequence(this)
                .AppendInterval(delayOpen)
                .Append(canvasGroup.DOFade(1f, 0.3f))
                .AppendCallback(() =>
                {
                    var delay = 0f;
                    foreach (var slot in saveSlots)
                    {
                        slot.canvasGroup.alpha = 0f;
                        slot.gameObject.SetActive(true);
                        slot.canvasGroup.DOFade(1f, 0.7f).SetDelay(delay).SetEase(Ease.InQuad);
                        delay += 0.1f;
                    }
                })
                .AppendInterval(0.5f)
                .AppendCallback(() => btnBackGraphic.DOFade(1f, 0.3f));
        }

        private Tween DoClose(float closeDelay)
        {
            DOTween.Kill(this);
            if (!btnBack.TryGetComponent<CanvasGroup>(out var btnBackGraphic))
                btnBackGraphic = btnBack.AddComponent<CanvasGroup>();
            return DOTween.Sequence(this)
                .AppendInterval(closeDelay)
                .Append(btnBackGraphic.DOFade(0f, 0.3f))
                .AppendCallback(() =>
                {
                    var delay = 0f;
                    for (var i = saveSlots.Length - 1; i >= 0; i--)
                    {
                        var cacheIndex = i;
                        saveSlots[i].canvasGroup.DOFade(0f, 0.5f).SetDelay(delay).SetEase(Ease.OutQuad);
                        delay += 0.1f;
                    }
                })
                .AppendInterval(0.3f)
                .Append(canvasGroup.DOFade(0f, 0.7f).SetEase(Ease.InQuad))
                .AppendCallback(() =>
                {
                    btnBack.interactable = true;
                    foreach (var slot in saveSlots)
                    {
                        slot.gameObject.SetActive(false);
                    }
                    gameObject.SetActive(false);
                });
        }
    }
}