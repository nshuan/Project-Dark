using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public interface IActionEffectLogic
    {
        void Initialize();
        void TriggerEffect(int effectId, Vector2 center, float size, float value);
    }
}