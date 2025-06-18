using System;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public abstract class EnemySpawnBehaviour : ScriptableObject
    {
        public void Spawn(Action completeCallback)
        {
            DOTween.Kill(this);
            DoSpawn().OnComplete(() => completeCallback?.Invoke()).SetTarget(this);
        }

        protected abstract Tween DoSpawn();
    }
}