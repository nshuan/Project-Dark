using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveDashToTower : IMoveTowersLogic
    {
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float duration;
        
        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            character.PlayDashEffect(endPos - startPos);
            
            var timeElapsed = 0f;
            while (timeElapsed / duration < 1f)
            {
                timeElapsed += Time.deltaTime;
                var speed = speedCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
                character.transform.position = Vector2.Lerp(startPos, endPos, speed);
                yield return null;
            }
                
            character.transform.position = endPos;
            character.StopDashEffect();
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}