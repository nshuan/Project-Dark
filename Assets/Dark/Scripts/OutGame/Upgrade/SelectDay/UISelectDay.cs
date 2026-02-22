using System;
using System.Collections.Generic;
using Dark.Scripts.Analytics;
using Dark.Scripts.ForDemo;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Dark.Tools.Language.Runtime;
using Data;
using DG.Tweening;
using InGame;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private Image imgLineFull;
        [SerializeField] private Image imgLineShort;
        [SerializeField] private TextMeshProUGUI txtTotalDay;
        [SerializeField] private RectTransform rectTotalDay;

        [SerializeField] private Vector3 offsetOnHideButtons = new Vector3(30f, 0f, 0f);
        [SerializeField] private float durationShowEachButton = 0.2f;
        [SerializeField] private float delayEachButton = 0.1f;
        [SerializeField] private Vector2 rectTotalDayShowOffset;
        [SerializeField] private DemoBlockLevelButton btnWishlist;

        private Dictionary<GameObject, CanvasGroup> dictPointerFullButtons;
        private Dictionary<GameObject, CanvasGroup> dictPointerQuickButtons;
        private List<CanvasGroup> listShowQuickButtons;
        private Vector3 cacheExpandPosition;
        private Vector2 cacheLineFullSize;
        private Vector2 cacheLineShortSize;
        
        private void OnEnable()
        {
            SetupDayButtons();
            
            txtTotalDay.SetText($"{PlayerDataManager.Instance.Data.level}/{btnDaysFull.Length}");
            rectTotalDay.localPosition = Vector3.zero;
        }

        private void SetupDayButtons()
        {
            RemoveAllButtonActions();
            
            // Get all button pointers
            dictPointerFullButtons = new Dictionary<GameObject, CanvasGroup>();
            foreach (var btn in btnDaysFull)
            {
                var groupPointer = btn.transform.Find("groupPointer");
                if (!groupPointer.TryGetComponent<CanvasGroup>(out var groupPointerCvg))
                    groupPointerCvg = groupPointer.AddComponent<CanvasGroup>();
                if (groupPointer)
                {
                    dictPointerFullButtons[btn.gameObject] = groupPointerCvg;
                }
            }
            
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
                        if (DemoConfig.IsDemo && a > DemoConfig.MaxDemoLevel)
                        {
                            btnWishlist?.CheckShowDemoPopup();
                            return;
                        }
                        
                        RemoveAllButtonActions();
    #if UNITY_EDITOR
                        LevelManager.isLoadFromInit = true;
    #endif
                        this.DelayCall(0.5f, () =>
                        {
                            Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame);
                            Loading.Instance.onSceneLoaded += () =>
                            {
                                LevelManager.Instance.LoadLevel(a);
                            };
                        });
                
                        LogManager.Log(LogConst.EventLogStartLevel, $"level_{a}", "from upgrade");
                    }
                });
                
                btn.GetComponentInChildren<TextMeshProUGUI>().SetTextLanguage("key_day", ("%{value}", a.ToString()));

                var groupBlock = btn.transform.Find("groupBlock");
                if (groupBlock)
                {
                    if (a < PlayerDataManager.Instance.Data.level + 1)
                        groupBlock.gameObject.SetActive(true);
                    else groupBlock.gameObject.SetActive(false);
                }
                
                var groupLock = btn.transform.Find("groupLock");
                if (groupLock)
                {
                    if (a > PlayerDataManager.Instance.Data.level + 1)
                        groupLock.gameObject.SetActive(true);
                    else groupLock.gameObject.SetActive(false);
                }
                
                if (dictPointerFullButtons.TryGetValue(btn.gameObject, out var groupPointer))
                {
                    if (a == PlayerDataManager.Instance.Data.level + 1)
                    {
                        groupPointer.transform.SetParent(btn.transform.parent.parent);
                        groupPointer.transform.SetSiblingIndex(1);
                        groupPointer.gameObject.SetActive(true);
                    }
                    else groupPointer.gameObject.SetActive(false);
                }
            }
            
            // Get quick buttons pointer
            dictPointerQuickButtons = new Dictionary<GameObject, CanvasGroup>();
            foreach (var btn in btnDayShort)
            {
                var groupPointer = btn.transform.Find("groupPointer");
                if (!groupPointer.TryGetComponent<CanvasGroup>(out var groupPointerCvg))
                    groupPointerCvg = groupPointer.AddComponent<CanvasGroup>();
                if (groupPointer)
                {
                    dictPointerQuickButtons[btn.gameObject] = groupPointerCvg;
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
                
                // Nếu chỉ hiện duy nhất 1 nút thì không cần hiện pointer
                if (dictPointerQuickButtons.TryGetValue(cvg.gameObject, out var groupPointer))
                {
                    if (a > 1 && a == PlayerDataManager.Instance.Data.level + 1)
                    {
                        groupPointer.transform.SetParent(cvg.transform.parent.parent);
                        groupPointer.transform.SetSiblingIndex(1);
                        groupPointer.gameObject.SetActive(true);
                    }
                    else groupPointer.gameObject.SetActive(false);
                }
                
                cvg.transform.parent.gameObject.SetActive(true);
                
                if (cvg.TryGetComponent<Button>(out var button))
                {
                    button.onClick.AddListener(() =>
                    {
                        if (a >= 1 && a <= PlayerDataManager.Instance.Data.level + 1)
                        {
                            if (DemoConfig.IsDemo && a > DemoConfig.MaxDemoLevel)
                            {
                                btnWishlist?.CheckShowDemoPopup();
                                return;
                            }
                            
                            RemoveAllButtonActions();
    #if UNITY_EDITOR
                            LevelManager.isLoadFromInit = true;
    #endif
                            this.DelayCall(0.5f, () =>
                            {
                                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame);
                                Loading.Instance.onSceneLoaded += () =>
                                {
                                    LevelManager.Instance.LoadLevel(a);
                                };
                            });
                
                            LogManager.Log(LogConst.EventLogStartLevel, $"level_{a}", "from upgrade");
                        }
                    });
                    
                    button.GetComponentInChildren<TextMeshProUGUI>()
                        .SetTextLanguage("key_day", ("%{value}", a.ToString()));
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

                    DOTween.Kill(imgLineFull, true);
                    imgLineFull.SetAlpha(1f);
                    imgLineFull.rectTransform.sizeDelta = cacheLineFullSize;
                    groupBtnFull.gameObject.SetActive(true);
                    groupBtnShort.gameObject.SetActive(false);
                });
            }

            cacheExpandPosition = btnExpand.transform.localPosition;
            cacheLineFullSize = imgLineFull.rectTransform.sizeDelta;
            cacheLineShortSize = new Vector2(imgLineShort.rectTransform.sizeDelta.x, imgLineShort.rectTransform.sizeDelta.y * (listShowQuickButtons.Count - 1));
            
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
                if (dictPointerFullButtons.TryGetValue(btn.gameObject, out var groupPointer))
                {
                    groupPointer.alpha = 1f;
                }
            }
            imgLineFull.SetAlpha(0f);
            imgLineFull.rectTransform.sizeDelta = cacheLineFullSize;
            
            var index = 0;
            foreach (var btn in listShowQuickButtons)
            {
                DOTween.Kill(btn);
                btn.alpha = 0f;
                btn.transform.localPosition = offsetOnHideButtons;
                btn.gameObject.SetActive(true);
                btn.DOFade(1f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn)
                    .SetDelay(delayEachButton * index);
                btn.transform.DOLocalMove(Vector3.zero, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn)
                    .SetDelay(delayEachButton * index);

                if (listShowQuickButtons.Count - index - 1 == PlayerDataManager.Instance.Data.level)
                {
                    if (dictPointerQuickButtons.TryGetValue(btn.gameObject, out var groupPointer))
                    {
                        groupPointer.alpha = 0f;
                        groupPointer.DOFade(1f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn)
                            .SetDelay(delayEachButton * (index + 2));
                    }
                }
                
                index += 1;
            }

            DOTween.Kill(btnExpand);
            btnExpand.alpha = 0f;
            btnExpand.transform.localPosition = cacheExpandPosition + offsetOnHideButtons;
            btnExpand.gameObject.SetActive(true);
            btnExpand.DOFade(1f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand)
                .SetDelay(delayEachButton * index);
            btnExpand.transform.DOLocalMove(cacheExpandPosition, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btnExpand)
                .SetDelay(delayEachButton * index);
            
            DOTween.Kill(imgLineShort);
            imgLineShort.SetAlpha(0f);
            imgLineShort.rectTransform.sizeDelta = new Vector2(cacheLineShortSize.x, 0f);
            imgLineShort.DOFade(1f, durationShowEachButton + delayEachButton * (listShowQuickButtons.Count - 1)).SetTarget(imgLineShort);
            if (listShowQuickButtons.Count > 1)
            {
                imgLineShort.rectTransform
                    .DOSizeDelta(cacheLineShortSize, (durationShowEachButton + delayEachButton * (listShowQuickButtons.Count - 1)))
                    .SetEase(Ease.Unset).SetTarget(imgLineShort);
            }
            
            groupBtnShort.SetActive(true);

            DOTween.Kill(rectTotalDay);
            rectTotalDay.DOLocalMoveX(rectTotalDayShowOffset.x, durationShowEachButton).SetEase(Ease.OutQuad)
                .SetTarget(rectTotalDay);
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

                    if (index == PlayerDataManager.Instance.Data.level)
                    {
                        if (dictPointerFullButtons.TryGetValue(btn.gameObject, out var groupPointer))
                        {
                            groupPointer.DOFade(0f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn);
                        }
                    }
                    
                    index += 1;
                }

                imgBackground.DOFade(0f, durationShowEachButton).SetTarget(imgBackground).SetDelay(delayEachButton * index)
                    .OnComplete(() => imgBackground.gameObject.SetActive(false));

                DOTween.Kill(imgLineFull);
                imgLineFull.DOFade(0f, durationShowEachButton + delayEachButton * index).SetTarget(imgLineFull);
                imgLineFull.rectTransform
                    .DOSizeDelta(new Vector2(cacheLineFullSize.x, 0f), (durationShowEachButton + delayEachButton * index))
                    .SetEase(Ease.Unset).SetTarget(imgLineFull);
            }
            else
            {
                index = listShowQuickButtons.Count;
                
                imgBackground.DOFade(0f, durationShowEachButton).SetTarget(imgBackground).SetDelay(delayEachButton * index)
                    .OnComplete(() => imgBackground.gameObject.SetActive(false));
                
                foreach (var btn in listShowQuickButtons)
                {
                    DOTween.Kill(btn);
                    
                    if (index == listShowQuickButtons.Count)
                    {
                        if (dictPointerQuickButtons.TryGetValue(btn.gameObject, out var groupPointer))
                        {
                            groupPointer.DOFade(0f, durationShowEachButton).SetEase(Ease.OutQuad).SetTarget(btn);
                        }
                    }
                    
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
                
                DOTween.Kill(imgLineShort);
                imgLineShort.DOFade(0f, durationShowEachButton + delayEachButton * (listShowQuickButtons.Count - 1)).SetTarget(imgLineShort).SetDelay(delayEachButton);
                imgLineShort.rectTransform
                    .DOSizeDelta(new Vector2(cacheLineShortSize.x, 0f), (durationShowEachButton + delayEachButton * (listShowQuickButtons.Count - 1)))
                    .SetEase(Ease.Unset).SetTarget(imgLineShort).SetDelay(delayEachButton);
            }
            
            DOTween.Kill(rectTotalDay);
            rectTotalDay.DOLocalMove(Vector3.zero, durationShowEachButton).SetEase(Ease.InQuad)
                .SetTarget(rectTotalDay);
        }
    }
}