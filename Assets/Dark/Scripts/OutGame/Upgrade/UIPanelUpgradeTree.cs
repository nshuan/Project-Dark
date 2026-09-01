using System.Collections;
using Coffee.UIExtensions;
using Dark.Scripts.AudioV2;
using Dark.Scripts.ForDemo;
using Dark.Scripts.SceneNavigation;
using Data;
using DG.Tweening;
using InGame.CharacterClass;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIPanelUpgradeTree : MonoBehaviour
    {
        [SerializeField] private UIHighlightNodeByCost nodeHighlight;
        [SerializeField] private Button btnBack;
        [SerializeField] private CanvasGroup btnWishlist;
        [SerializeField] private CanvasGroup btnFeedback;
        [SerializeField] private CanvasGroup btnWishlistNewGame;
        [SerializeField] private Transform treeParent;
        [SerializeField] private UIParticle vfxSpawn;
        [SerializeField] private CanvasGroup groupUIHiddenOnSpawn;
        [SerializeField] private ZoomInOut treeZoom;
        [SerializeField] private float delaShowUI;
        [SerializeField] private string cueSpawnTree;
        [SerializeField] private string cueSpawnNodes;

        [Space] [Header("Settings")] 
        [SerializeField] private GameObject popupSettings;

        private bool treeSpawned;
        private UIUpgradeTree tree;
        public UIUpgradeTree Tree => tree;
        public CanvasGroup TreeCvg { get;private set; }
        private Vector3 offsetOnHideDemoButtons = new Vector3(30f, 0f, 0f);
        private float durationShowEachDemoButtons = 0.2f;
        private float delayEachDemoButtons = 0.1f;

        private void Awake()
        {
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(() =>
            {
                btnBack.interactable = false;
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(IESpawn());
        }

        private void Update()
        {
            if (treeSpawned)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    popupSettings.SetActive(true);
                }
            }
        }

        private IEnumerator IESpawn()
        {
            treeSpawned = false;
            var delaySpawn = Loading.Instance.CurrentTotalDurationAfterSceneLoaded;
            groupUIHiddenOnSpawn.alpha = 0f;
            vfxSpawn.gameObject.SetActive(false);
            treeZoom.SetZoom(0.7f);
            yield return new WaitForSeconds(delaySpawn);
            // AudioManagerV2.Instance.PlayInGame(cueSpawnTree);
            AudioManagerV2.Instance.PlayInGame(cueSpawnNodes);
            treeZoom.ZoomTo(0.7f, ((RectTransform)transform).position, 0.5f, 0.3f);
            if (!GameConst.HideLaserWaveOnSpawnTree)
                vfxSpawn.gameObject.SetActive(true);
            yield return new WaitForSeconds(delaShowUI);
            yield return groupUIHiddenOnSpawn.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();
            treeSpawned = true;
        }

        public void SpawnTree()
        {
            tree = Instantiate(UpgradeTreeManifest.GetTreePrefab((CharacterClass)PlayerDataManager.Instance.Data.characterClass), treeParent);
            if (Tree.TryGetComponent<CanvasGroup>(out var cvg))
                TreeCvg = cvg;
            else
                TreeCvg = tree.AddComponent<CanvasGroup>();
            // tree.OnNodeUpgraded += (node) => scrollView.FocusTo((RectTransform)node.transform);
            nodeHighlight.InitNodesByCost(tree);
        }

        public void ShowDemoButtons()
        {
            if (!DemoConfig.IsDemo) return;
            
            DOTween.Kill(btnWishlist, true);
            DOTween.Kill(btnFeedback, true);

            var btnFeedBackPosition = btnFeedback.transform.localPosition;
            btnFeedback.alpha = 0f;
            btnFeedback.transform.localPosition = btnFeedBackPosition + offsetOnHideDemoButtons;
            btnFeedback.gameObject.SetActive(true);
            btnFeedback.DOFade(1f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnFeedback);
            btnFeedback.transform.DOLocalMove(btnFeedBackPosition, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnFeedback);
            
            var btnWishlistPosition = btnWishlist.transform.localPosition;
            btnWishlist.alpha = 0f;
            btnWishlist.transform.localPosition = btnWishlistPosition + offsetOnHideDemoButtons;
            btnWishlist.gameObject.SetActive(true);
            btnWishlist.DOFade(1f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlist).SetDelay(delayEachDemoButtons);
            btnWishlist.transform.DOLocalMove(btnWishlistPosition, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlist).SetDelay(delayEachDemoButtons);
        }

        public void HideDemoButtons()
        {
            if (!DemoConfig.IsDemo) return;
            
            DOTween.Kill(btnWishlist, true);
            DOTween.Kill(btnFeedback, true);
            
            btnFeedback.DOFade(0f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnFeedback).SetDelay(delayEachDemoButtons);
            btnFeedback.transform.DOLocalMove(btnFeedback.transform.localPosition + offsetOnHideDemoButtons, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnFeedback).SetDelay(delayEachDemoButtons)
                .OnComplete(() =>
                {
                    btnFeedback.gameObject.SetActive(false);
                    btnFeedback.transform.localPosition -= offsetOnHideDemoButtons;
                });
            btnWishlist.DOFade(0f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlist);
            btnWishlist.transform.DOLocalMove(btnWishlist.transform.localPosition + offsetOnHideDemoButtons, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlist)
                .OnComplete(() =>
                {
                    btnWishlist.gameObject.SetActive(false);
                    btnWishlist.transform.localPosition -= offsetOnHideDemoButtons;
                });
        }
        
        public void ShowWishlistNewGameButtons()
        {
            DOTween.Kill(btnWishlistNewGame, true);

            var btnWishlistNewGamePosition = btnWishlistNewGame.transform.localPosition;
            btnWishlistNewGame.alpha = 0f;
            btnWishlistNewGame.transform.localPosition = btnWishlistNewGamePosition + offsetOnHideDemoButtons;
            btnWishlistNewGame.gameObject.SetActive(true);
            btnWishlistNewGame.DOFade(1f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlistNewGame);
            btnWishlistNewGame.transform.DOLocalMove(btnWishlistNewGamePosition, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlistNewGame);
        }

        public void HideWishlistNewGameButtons()
        {
            DOTween.Kill(btnWishlistNewGame, true);
            
            btnWishlistNewGame.DOFade(0f, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlistNewGame).SetDelay(delayEachDemoButtons);
            btnWishlistNewGame.transform.DOLocalMove(btnWishlistNewGame.transform.localPosition + offsetOnHideDemoButtons, durationShowEachDemoButtons).SetEase(Ease.OutQuad).SetTarget(btnWishlistNewGame).SetDelay(delayEachDemoButtons)
                .OnComplete(() =>
                {
                    btnWishlistNewGame.gameObject.SetActive(false);
                    btnWishlistNewGame.transform.localPosition -= offsetOnHideDemoButtons;
                });
        }
    }
}