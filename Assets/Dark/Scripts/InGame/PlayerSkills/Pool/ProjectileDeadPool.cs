using System.Collections;
using Core;
using UnityEngine;

namespace InGame
{
    public class ProjectileDeadPool : Pool<Transform, ProjectileDeadPool>
    {
        [SerializeField] private float delayDisappear = 8f;
        
        public Transform Get(Vector2 direction, float overrideDelayDisappear = -1)
        {
            if (overrideDelayDisappear <= 0) overrideDelayDisappear = delayDisappear; 
            var obj = Get(null);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            obj.GetChild(0).rotation = Quaternion.Euler(0f, 0f, angle);
            obj.gameObject.SetActive(true);
            StartCoroutine(IERelease(obj, overrideDelayDisappear));
            return obj;
        }

        private IEnumerator IERelease(Transform obj, float delayDisappear)
        {
            yield return new WaitForSeconds(delayDisappear);
            Release(obj);
        }
    }
}