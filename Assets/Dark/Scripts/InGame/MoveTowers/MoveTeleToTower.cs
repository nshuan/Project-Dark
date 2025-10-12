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

        public void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            
        }

        public IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete)
        {
            yield return character.PLayTeleEffect(toTower.transform.position + toTower.GetTowerHeight()).WaitForCompletion();
            
            character.transform.position = toTower.transform.position + toTower.GetTowerHeight();
            yield return new WaitForEndOfFrame();
            
            yield return character.StopTeleEffect().WaitForCompletion();
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}