using System;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public interface IUpgradeNodeSpawnLogic
    {
        Tween DoSpawn();
    }

    [Serializable]
    public class NodeSpawnDefault : IUpgradeNodeSpawnLogic
    {
        public Tween DoSpawn()
        {
            return DOTween.Sequence();
        }
    }
    
    [Serializable]
    public class NodeSpawnFlash : IUpgradeNodeSpawnLogic
    {
        [SerializeField] private CanvasGroup groupNode;
        [SerializeField] private Image imgFlash;
        
        public Tween DoSpawn()
        {
            groupNode.alpha = 0f;
            
            return DOTween.Sequence()
                .AppendCallback(() =>
                {
                    imgFlash.gameObject.SetActive(true);
                    imgFlash.SetAlpha(1f);
                    groupNode.alpha = 1f;
                })
                .Append(imgFlash.DOFade(0f, 0.3f).SetEase(Ease.OutQuad))
                .AppendCallback(() => imgFlash.gameObject.SetActive(false));
        }
    }
    
    [Serializable]
    public class NodeSpawnRotateAndGlow : IUpgradeNodeSpawnLogic
    {
        [SerializeField] private Image imgBorder;
        [SerializeField] private UIParticle vfxSpawn;
        
        public Tween DoSpawn()
        {
            imgBorder.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            imgBorder.transform.localScale = Vector3.one;
            
            return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    DOVirtual.DelayedCall(0.2f, () => vfxSpawn.gameObject.SetActive(true));
                })
                .Append(imgBorder.transform.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f))
                .Join(imgBorder.transform.DOScale(1.1f, 0.4f).SetEase(Ease.OutQuad))
                .Append(imgBorder.transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
        }
    }
    
    [Serializable]
    public class NodeSpawnGlowVfx : IUpgradeNodeSpawnLogic
    {
        [SerializeField] private CanvasGroup groupNode;
        [SerializeField] private UIParticle vfxSpawn;
        [SerializeField] private bool autoPlay = false;
        
        public Tween DoSpawn()
        {
            groupNode.alpha = 0f;
            
            return DOTween.Sequence()
                .AppendCallback(() =>
                {
                    vfxSpawn.gameObject.SetActive(true);
                    groupNode.alpha = 1f;
                    if (!autoPlay) 
                        vfxSpawn.Play();
                });
        }
    }
}