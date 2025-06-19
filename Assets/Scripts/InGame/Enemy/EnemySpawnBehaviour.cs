using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public abstract class EnemySpawnBehaviour : ScriptableObject
    {
        public virtual void Init(Transform enemy) { }
        public abstract Tween DoSpawn(Transform enemy);
    }
}