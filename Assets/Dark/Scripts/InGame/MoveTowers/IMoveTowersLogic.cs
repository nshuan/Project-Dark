using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public interface IMoveTowersLogic
    {
        void SetStats(int damage, float stagger, int maxHitEachTrigger, float size);
        void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size);
        IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete = null);
    }
}