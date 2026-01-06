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
            // calculate target z-rotation
            var targetRotation =
                new Vector3(0f, 0f, (Mathf.RoundToInt(imgBorder.transform.rotation.z / 180f) + 1) * 180f);
            imgBorder.transform.localScale = Vector3.one;
            vfxSpawn.gameObject.SetActive(false);

            return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    vfxSpawn.gameObject.SetActive(true);
                })
                .AppendInterval(0.8f)
                .AppendCallback(() =>
                {
                    imgBorder.transform.DOLocalRotate(targetRotation, 0.4f).SetRelative()
                        .OnComplete(() =>
                        {
                            imgBorder.transform.localRotation = Quaternion.Euler(targetRotation);
                        });
                });
            // .Append(imgBorder.transform.DOScale(1.1f, 0.4f).SetEase(Ease.OutQuad))
            // .Append(imgBorder.transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
        }
    }
    
    [Serializable]
    public class NodeSpawnGlowVfx : IUpgradeNodeSpawnLogic
    {
        [SerializeField] private CanvasGroup groupNode;
        [SerializeField] private bool autoPlay = false;
        [SerializeField] private UpgradeNodeType nodeType;
        
        public Tween DoSpawn()
        {
            groupNode.alpha = 0f;
            
            return DOTween.Sequence()
                .AppendCallback(() =>
                {
                    var vfxSpawn = GetVfxAppear();
                    vfxSpawn.gameObject.SetActive(true);
                    groupNode.alpha = 1f;
                    if (!autoPlay) 
                        vfxSpawn.Play();
                    
                    DOVirtual.DelayedCall(1f, () => ReleaseVfxAppear(vfxSpawn));
                });
        }
        
        private UIParticle GetVfxAppear()
        {
            return nodeType switch
            {
                UpgradeNodeType.NodeSkill => UIUpgradeNodeSkillPool.Instance.GetVfxAppear(groupNode.transform, true),
                UpgradeNodeType.NodeEffect => UIUpgradeNodeEffectPool.Instance.GetVfxAppear(groupNode.transform, true),
                UpgradeNodeType.NodeStat => UIUpgradeNodeStatPool.Instance.GetVfxAppear(groupNode.transform, true),
                UpgradeNodeType.NodeStat2 => UIUpgradeNodeStat2Pool.Instance.GetVfxAppear(groupNode.transform, true),
                _ => null
            };
        }

        private void ReleaseVfxAppear(UIParticle vfx)
        {
            switch (nodeType)
            {
                case UpgradeNodeType.NodeSkill:
                    UIUpgradeNodeSkillPool.Instance.ReleaseVfxAppear(vfx);
                    break;
                case UpgradeNodeType.NodeEffect:
                    UIUpgradeNodeEffectPool.Instance.ReleaseVfxAppear(vfx);
                    break;
                case UpgradeNodeType.NodeStat:
                    UIUpgradeNodeStatPool.Instance.ReleaseVfxAppear(vfx);
                    break;
                case UpgradeNodeType.NodeStat2:
                    UIUpgradeNodeStat2Pool.Instance.ReleaseVfxAppear(vfx);
                    break;
            }
        }
    }
}