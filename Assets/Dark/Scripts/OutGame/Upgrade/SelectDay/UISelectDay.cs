using System;
using System.Collections.Generic;
using Dark.Scripts.Analytics;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using DG.Tweening;
using InGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.SelectDay
{
    public class UISelectDay : MonoBehaviour
    {
        [SerializeField] private Image imgBackground;
        [SerializeField] private GameObject groupBtnShort;
        [SerializeField] private GameObject groupBtnFull;
        [SerializeField] private Button[] btnDaysFull;
        [SerializeField] private CanvasGroup[] btnDayShort;
        [SerializeField] private CanvasGroup btnExpand;

        [SerializeField] private Vector3 offsetOnHideButtons = new Vector3(30f, 0f, 0f);
        [SerializeField] private float durationShowEachButton = 0.2f;
        [SerializeField] private float delayEachButton = 0.1f;

        private List<CanvasGroup> listShowQuickButtons;
        private Vector3 cacheExpandPosition;
        
        private void OnEnable()
        {
            SetupDayButtons();
        }

        private void SetupDayButtons()
        {
            RemoveAllButtonActions();
            
            // Setup buttons in full list
            var index = 0;
            foreach (var btn in btnDaysFull)
            {
                index += 1;
                var a = index;
                btn.onClick.AddListener(() =>
                {
                    if (a >= 1 && a <= PlayerDataManager.Instance.Data.level + 1)
                    {
                        RemoveAllButtonActions();
    #if UNITY_EDITOR
                        LevelManager.isLoadFromInit = true;
    #endif
                        this.DelayCall(0.5f, () =>
                        {
                            Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                            {
                                LevelManager.Instance.LoadLevel(a);
                            });
                        });
                
                        LogManager.Log(LogConst.EventLogStartLevel, $"level_{a}", "from upgrade");
                    }
                });
                
                btn.GetComponentInChildren<TextMeshProUGUI>()
                    .SetText($"Day {a}");

                var groupBlock = btn.transform.Find("groupBlock");
                if (groupBlock)
                {
                    if (a == PlayerDataManager.Instance.Data.level + 1)
                        groupBlock.gameObject.SetActive(false);
                    else groupBlock.gameObject.SetActive(true);
                }
            }
            
            // Setup buttons in short list
            index = PlayerDataManager.Instance.Data.level + 1;
            listShowQuickButtons = new List<CanvasGroup>();
            foreach (var cvg in btnDayShort)
            {
                if (index <= 0)
                {
                    cvg.transform.parent.gameObject.SetActive(false);
                    continue;
                }

                var a = index;
                
                var groupBlock = cvg.transform.Find("groupBlock");
                if (groupBlock)
                {
                    if (a == PlayerDataManager.Instance.Data.level + 1)
                        groupBlock.gameObject.SetActive(false);
                    else groupBlock.gameObject.SetActive(true);
                }
                
                cvg.transform.parent.gameObject.SetActive(true);
                
                if (cvg.TryGetComponent<Button>(out var button))
                {
                    button.onClick.AddListener(() =>
                    {
                        if (a >= 1 && a <= PlayerDataManager.Instance.Data.level + 1)
                        {
                            RemoveAllButtonActions();
    #if UNITY_EDITOR
                            LevelManager.isLoadFromInit = true;
    #endif
                            this.DelayCall(0.5f, () =>
                            {
                                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                                {
                                    LevelManager.Instance.LoadLevel(a);
                                });
                            });
                
                            LogManager.Log(LogConst.EventLogStartLevel, $"level_{a}", "from upgrade");
                        }
                    });
                    
                    button.GetComponentInChildren<TextMeshProUGUI>()
                        .SetText($"Day {a}");
                }
                
                listShowQuickButtons.Add(cvg);
                
                index -= 1;
            }

            if (btnExpand.TryGetComponent<Button>(out var buttonExpand))
            {
                buttonExpand.onClick.RemoveAllListeners();
                buttonExpand.onClick.AddListener(() =>
                {
                    foreach (var btn in btnDaysFull)
                    {
                        DOTween.Kill(btn, true);
                        btn.gameObject.SetActive(true);
                    }
                    groupBtnFull.gameObject.SetActive(true);
                    groupBtnShort.gameObject.SetActive(false);
                });
            }

            cacheExpandPosition = btnExpand.transform.localPosition;
            
            imgBackground.gameObject.SetActive(false);
        }

        private void RemoveAllButtonActions()
        {
            foreach (var btn in btnDaysFull)
            {
                btn.onClick.RemoveAllListeners();
            }

            foreach (var cvg in btnDayShort)
            {
                if (cvg.TryGetComponent<Button>(out var button))
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }
        
        public void ShowButtons()
        {
            DOTween.Kill(imgBackground);
            imgBackground.SetAlpha(0f);
            imgBackground.gameObject.SetActive(true);
            imgBackground.DOFade(1f, durationShowEachButton).SetTarget(imgBackground);
            
            groupBtnFull.SetActive(false);
            foreach (var btn in btnDaysFull)
            {
                btn.gameObject.SetActive(true);
            }
            
            var index = 0;
            foreach (var btn in listShowQuickButtons)
            {
                DOTween.Kill(btn);
                btn.alpha = 0f;
                btn.transform.localPosition = offsetOnHideButtons;
                btn.gameObject.SetActive(true);
                btn.DOFade(1f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn)
                    .SetDelay(delayEachButton * index);
                btn.transform.DOLocalMove(Vector3.zero, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn);
                index += 1;
            }

            DOTween.Kill(btnExpand);
            btnExpand.alpha = 0f;
            btnExpand.transform.localPosition = cacheExpandPosition + offsetOnHideButtons;
            btnExpand.gameObject.SetActive(true);
            btnExpand.DOFade(1f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand)
                .SetDelay(delayEachButton * index);
            btnExpand.transform.DOLocalMove(cacheExpandPosition, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand);
            
            groupBtnShort.SetActive(true);
        }

        public void HideButtons()
        {
            DOTween.Kill(imgBackground);
            
            var index = 0;
            if (groupBtnFull.activeInHierarchy)
            {
                foreach (var btn in btnDaysFull)
                {
                    DOTween.Kill(btn);
                    if (btn.TryGetComponent<CanvasGroup>(out var cvg))
                        cvg.DOFade(0f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn).SetDelay(delayEachButton * index);
                    btn.transform.DOLocalMove(offsetOnHideButtons, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn).SetDelay(delayEachButton * index)
                        .OnComplete(() =>
                        {
                            btn.gameObject.SetActive(false);
                            btn.transform.localPosition = Vector3.zero;
                            cvg.alpha = 1f;
                        });

                    index += 1;
                }

                imgBackground.DOFade(0f, durationShowEachButton).SetTarget(imgBackground).SetDelay(delayEachButton * index)
                    .OnComplete(() => imgBackground.gameObject.SetActive(false));
            }
            else
            {
                index = listShowQuickButtons.Count;
                
                imgBackground.DOFade(0f, durationShowEachButton).SetTarget(imgBackground).SetDelay(delayEachButton * index)
                    .OnComplete(() => imgBackground.gameObject.SetActive(false));
                
                foreach (var btn in listShowQuickButtons)
                {
                    DOTween.Kill(btn);
                    btn.DOFade(0f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn).SetDelay(delayEachButton * index);
                    btn.transform.DOLocalMove(offsetOnHideButtons, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn).SetDelay(delayEachButton * index)
                        .OnComplete(() =>
                        {
                            btn.gameObject.SetActive(false);
                            btn.transform.localPosition = Vector3.zero;
                        });
                    index -= 1;
                }
                
                DOTween.Kill(btnExpand);
                btnExpand.DOFade(0f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand);
                btnExpand.transform.DOLocalMove(cacheExpandPosition + offsetOnHideButtons, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand)
                    .OnComplete(() =>
                    {
                        btnExpand.gameObject.SetActive(false);
                        btnExpand.transform.localPosition = cacheExpandPosition;
                    });
            }
        }
    }
}