using System;
using System.Collections;
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
            // Jump up
            var jumpPeak = startPos + new Vector2(0f, 2f);
            while (Vector2.Distance(character.transform.position, jumpPeak) > 0.2f)
            {
                character.transform.position = Vector2.Lerp(character.transform.position, jumpPeak, Time.deltaTime * 4f);
                yield return null;
            }
            
            character.transform.position = endPos;
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}