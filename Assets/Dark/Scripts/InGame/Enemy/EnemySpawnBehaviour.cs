using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public abstract class EnemySpawnBehaviour : ScriptableObject
    {
        public virtual void Init(EnemyEntity enemy) { }
        public abstract Tween DoSpawn(EnemyEntity enemy);
    }
}