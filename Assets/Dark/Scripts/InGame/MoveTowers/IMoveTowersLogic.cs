using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public interface IMoveTowersLogic
    {
        void SetStats(int damage, float stagger, int maxHitEachTrigger, float size);
        IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete = null);
    }
}