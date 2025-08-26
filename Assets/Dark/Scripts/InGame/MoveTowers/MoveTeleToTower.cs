using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveTeleToTower : IMoveTowersLogic
    {
        public void SetStats(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            
        }

        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            yield return character.PLayTeleEffect(endPos).WaitForCompletion();
            
            character.transform.position = endPos;
            yield return new WaitForEndOfFrame();
            
            yield return character.StopTeleEffect().WaitForCompletion();
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}