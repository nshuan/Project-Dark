using System.Collections;
using Core;
using UnityEngine;

namespace InGame
{
    public class ProjectileHomingDeadPool : Pool<HomingProjectileDead, ProjectileHomingDeadPool>
    {
        [SerializeField] private float delayDisappear = 8f;
        
        public HomingProjectileDead Get(Vector2 position, Vector2 direction, float overrideDelayDisappear = -1)
        {
            if (overrideDelayDisappear <= 0) overrideDelayDisappear = delayDisappear; 
            var obj = Get(null);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            obj.visual.rotation = Quaternion.Euler(0f, 0f, angle);
            obj.transform.position = position;
            if (ProjectileDeadPool.IsInAnyGate(position))
            {
                overrideDelayDisappear = 0.5f;
            }
            obj.Reset();
            obj.gameObject.SetActive(true);
            StartCoroutine(IERelease(obj, overrideDelayDisappear));
            return obj;
        }

        private IEnumerator IERelease(HomingProjectileDead obj, float delayDisappear)
        {
            yield return new WaitForSeconds(delayDisappear);
            yield return new WaitForSeconds(obj.Dispose());
            Release(obj);
            obj.Reset();
        }
    }
}