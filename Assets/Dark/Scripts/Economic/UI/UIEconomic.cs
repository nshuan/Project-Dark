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
            var stepSize = 1;
            var step = (int)(maxUpdateDuration / updateInterval);
            if (current < target)
            {
                if ((target - current) * updateInterval > maxUpdateDuration)
                    stepSize = (int)((target - current) / maxUpdateDuration * updateInterval);
                else step = target - current;

                TweenCallback actionUpdate = () =>
                {
                    current += stepSize;
                    UpdateUI();
                };
                
                for (var i = 0; i < step; i++)
                {
                    seq.AppendCallback(actionUpdate)
                        .AppendInterval(updateInterval);
                }
                
            }
            else if (current > target)
            {
                if ((-target + current) * updateInterval > maxUpdateDuration)
                    stepSize = (int)((-target + current) / maxUpdateDuration * updateInterval);
                else step = -target + current;
                
                TweenCallback actionUpdate = () =>
                {
                    current -= stepSize;
                    UpdateUI();
                };
                
                for (var i = 0; i < step; i++)
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