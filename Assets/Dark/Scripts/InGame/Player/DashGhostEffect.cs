using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class DashGhostEffect : MonoBehaviour
    {
        public SpriteRenderer originalSprite;
        public Transform ghostPoolParent;
        public SpriteRenderer ghostPrefab;
        public float spawnRate = 0.02f;
        public float ghostLifespan = 0.3f;

        private bool canSpawn;
        private float timer;

        private Queue<SpriteRenderer> ghostPool = new Queue<SpriteRenderer>();
        
        public void DoEffect()
        {
            canSpawn = true;
            StartCoroutine(IESpawnGhost());
        }

        public void StopEffect()
        {
            canSpawn = false;
        }

        IEnumerator IESpawnGhost()
        {
            while (canSpawn)
            {
                Spawn();
                yield return new WaitForSeconds(spawnRate);
            }
        }
        
        private void Spawn()
        {
            if (!ghostPool.TryDequeue(out var ghost))
            {
                ghost = Instantiate(ghostPrefab);
            }
            ghost.transform.SetParent(null);
            ghost.transform.position = transform.position;
            ghost.sprite = originalSprite.sprite;
            ghost.gameObject.SetActive(true);
            StartCoroutine(FadeAndDestroy(ghost, ghostLifespan));
        }

        IEnumerator FadeAndDestroy(SpriteRenderer sr, float time)
        {
            Color color = sr.color;
            float t = 0;
            while (t < time)
            {
                float alpha = Mathf.Lerp(1, 0, t / time);
                sr.color = new Color(color.r, color.g, color.b, alpha);
                t += Time.deltaTime;
                yield return null;
            }
            sr.gameObject.SetActive(false);
            sr.transform.SetParent(ghostPoolParent);
            ghostPool.Enqueue(sr);
        }
    }
}