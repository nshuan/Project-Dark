using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveFlashToTower : IMoveTowersLogic
    {
        [SerializeField] private float explodeSize = 2f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int damage;
        [SerializeField] private float stagger;
        
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private EnemyEntity hitTarget;
        private Transform characterTransform;
        
        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            characterTransform = character.transform;
            
            // Jump up
            var jumpPeak = startPos + new Vector2(0f, 2f);
            while (Vector2.Distance(character.transform.position, jumpPeak) > 0.2f)
            {
                character.transform.position = Vector2.Lerp(character.transform.position, jumpPeak, Time.deltaTime * 4f);
                yield return null;
            }
            
            character.transform.position = endPos;
            yield return new WaitForEndOfFrame();
            
            var count = Physics2D.CircleCastNonAlloc(character.transform.position, explodeSize, Vector2.zero, hits, 0f,
                enemyLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ExplosionHit(hits[i].transform, damage);
                }
            }
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void ExplosionHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.Damage((int)value, (hitTransform.position - characterTransform.position).normalized, stagger);
                }
            }
        }
    }
}