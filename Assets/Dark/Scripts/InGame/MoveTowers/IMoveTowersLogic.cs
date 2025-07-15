using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public interface IMoveTowersLogic
    {
        IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete = null);
    }
}