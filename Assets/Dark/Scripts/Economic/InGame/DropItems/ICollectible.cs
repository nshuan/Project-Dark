using System;
using Economic.InGame;
using UnityEngine;

namespace InGame
{
    public interface ICollectible
    {
        void Collect(EItemDropCollector target, float delay);
    }
}