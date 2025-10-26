using System.Collections;
using UnityEngine;

namespace Economic.UI
{
    public class UIEconomic : MonoBehaviour
    {
        private Coroutine coroutineAnimatedUpdating;
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
            
            if (coroutineAnimatedUpdating == null)
            {
                coroutineAnimatedUpdating = StartCoroutine(IEAnimatedUpdating());
            }
        }

        private IEnumerator IEAnimatedUpdating()
        {
            var step = 1;
            if (current < target)
            {
                if ((target - current) * updateInterval > maxUpdateDuration)
                    step = (int)((target - current) / maxUpdateDuration * updateInterval);
                
                while (current < target)
                {
                    current += step;
                    UpdateUI();
                    yield return new WaitForSeconds(updateInterval);
                }
            }
            else if (current > target)
            {
                if ((- target + current) * updateInterval > maxUpdateDuration)
                    step = (int)((- target + current) / maxUpdateDuration * updateInterval);
                
                while (current > target)
                {
                    current -= step;
                    UpdateUI();
                    yield return new WaitForSeconds(updateInterval);
                }
            }

            current = target;
            UpdateUI();
            coroutineAnimatedUpdating = null;
        }
    }
}