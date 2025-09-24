using System;
using UnityEngine;

namespace InGame
{
    public interface ICollectible
    {
        bool CanCollectByMouse { get; set; }
        void Collect(Transform target, float delay);
    }
}