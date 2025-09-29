using System.Collections;
using Core;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemySplashPool : Pool<Transform, EnemySplashPool>
    {
        public void GetAndRelease(Transform parent, Vector2 position, bool flip, float delaySpawn, float delayRelease)
        {
            StartCoroutine(IESpawnAndRelease(parent, position, flip, delaySpawn, delayRelease));
        }

        private IEnumerator IESpawnAndRelease(Transform parent, Vector2 position, bool flip, float delaySpawn, float delayRelease)
        {
            yield return new WaitForSeconds(delaySpawn);
            var vfx = Get(parent, true);
            vfx.position = position;
            vfx.localScale = new Vector2(flip ? -1 : 1, 1);
            yield return new WaitForSeconds(delayRelease);
            Release(vfx);
        }
    }
}