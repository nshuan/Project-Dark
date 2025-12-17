using System.Collections;
using Dark.Scripts.OutGame.Upgrade;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.ForDemo
{
    // Do not use UIButtonAnimated in the same game object with this 
    public class DemoBlockNodeButton : DemoBlockButton, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UIUpgradeNode node;
        [SerializeField] private GameObject extraHidden;
        [SerializeField] private GameObject nodeHoverField;
        
        protected Vector3 defaultScale;

        protected virtual void Awake()
        {
            defaultScale = node.transform.localScale;
            defaultScale.z = 1f;
        }

        protected override void Start()
        {
            if (!ShouldShowButton())
            {
                hiddenButton?.SetActive(true);
                gameObject.SetActive(false);
            }
            else
            {
                extraHidden.SetActive(false);
                nodeHoverField.SetActive(false);
                hiddenButton?.SetActive(false);
                buttonVisual.alpha = 1f;
            }
        }

        protected override bool ShouldShowButton()
        {
            if (node.config == null) return false;
            return DemoConfig.IsLockedNode(node.config.nodeId);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;
            
            extraHidden.SetActive(false);
            nodeHoverField.SetActive(false);
            
            DOTween.Kill(this);
            buttonVisual.alpha = 1f;
            hiddenButton?.SetActive(false);
            node.transform.localRotation = Quaternion.identity;
            node.transform.localScale = defaultScale;
            node.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.3f, 20, 0.1f).SetTarget(transform);
            node.transform.DOScale(new Vector3(0.2f, 0.2f, 0), 0.2f).SetRelative().SetEase(Ease.OutQuad);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;
            
            node.transform.DOScale(defaultScale, 0.2f).SetEase(Ease.InQuad);
        }

        private Coroutine coroutinePointerDown;
        private IEnumerator IEPointerDown()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true).SetRelative()
                .Append(node.transform.DOScale(new Vector3(-0.1f, -0.1f, -0.1f), 0.2f).SetEase(Ease.OutQuad)).Play();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(node.transform.DOScale(defaultScale + new Vector3(0.2f, 0.2f, 0), 0.2f)).Play();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;
            
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(node.transform.DOPunchScale(new Vector3(-0.1f, -0.1f, -0.1f), 0.2f))
                .OnComplete(() => base.OnPointerClick(eventData));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            coroutinePointerDown = StartCoroutine(IEPointerDown());
        }
    }
}