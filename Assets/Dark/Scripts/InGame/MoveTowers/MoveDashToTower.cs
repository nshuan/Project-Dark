using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveDashToTower : IMoveTowersLogic
    {
        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            character.PlayDashEffect(endPos - startPos);
            while (Vector2.Distance(character.transform.position, endPos) > 0.1f)
            {
                character.transform.position = Vector2.Lerp(character.transform.position, endPos, Time.deltaTime * 8f);
                yield return null;
            }
                
            character.transform.position = endPos;
            character.StopDashEffect();
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}