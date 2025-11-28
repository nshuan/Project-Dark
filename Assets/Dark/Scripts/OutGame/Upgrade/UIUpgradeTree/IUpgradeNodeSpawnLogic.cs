using System;
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
}