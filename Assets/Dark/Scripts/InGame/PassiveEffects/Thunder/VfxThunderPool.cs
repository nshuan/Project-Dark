using System.Collections;
using Core;
using UnityEngine;

namespace InGame
{
    public class VfxThunderPool : Pool<Transform, VfxThunderPool>
    {
        public void GetAndRelease(Transform parent, Vector2 position, float delaySpawn, float delayRelease)
        {
            StartCoroutine(IESpawnAndRelease(parent, position, delaySpawn, delayRelease));
        }

        private IEnumerator IESpawnAndRelease(Transform parent, Vector2 position, float delaySpawn, float delayRelease)
        {
            yield return new WaitForSeconds(delaySpawn);
            var vfx = Get(parent, true);
            vfx.position = position;
            yield return new WaitForSeconds(delayRelease);
            Release(vfx);
        }
    }
}