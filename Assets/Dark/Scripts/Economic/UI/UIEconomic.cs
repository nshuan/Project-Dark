using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Economic.UI
{
    public class UIEconomic : MonoBehaviour
    {
        protected int current;
        protected int target;
        protected float updateInterval = 0.05f;
        protected float maxUpdateDuration = 3f;

        public virtual void UpdateUI()
        {
            
        }
        
        public void AnimateUpdating(int target)
        {
            if (target == this.target) return;
            this.target = target;

            DoAnimatedUpdating().OnComplete(() =>
            {
                current = this.target;
                UpdateUI();
            });
        }

        private Tween DoAnimatedUpdating()
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            var step = 1;
            if (current < target)
            {
                if ((target - current) * updateInterval > maxUpdateDuration)
                    step = (int)((target - current) / maxUpdateDuration * updateInterval);

                TweenCallback actionUpdate = () =>
                {
                    current += step;
                    UpdateUI();
                };
                
                for (var i = 0; i < maxUpdateDuration / updateInterval; i++)
                {
                    seq.AppendCallback(actionUpdate)
                        .AppendInterval(updateInterval);
                }
                
            }
            else if (current > target)
            {
                if ((- target + current) * updateInterval > maxUpdateDuration)
                    step = (int)((- target + current) / maxUpdateDuration * updateInterval);
                
                TweenCallback actionUpdate = () =>
                {
                    current -= step;
                    UpdateUI();
                };
                
                for (var i = 0; i < maxUpdateDuration / updateInterval; i++)
                {
                    seq.AppendCallback(actionUpdate)
                        .AppendInterval(updateInterval);
                }
            }

            seq.AppendCallback(() =>
            {
                current = target;
                UpdateUI();
            });

            return seq;
        }
    }
}