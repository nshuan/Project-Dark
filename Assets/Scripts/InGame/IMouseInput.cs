using System;

namespace InGame
{
    public interface IMouseInput : IDisposable
    {
        InputInGame InputManager { get; set; }
        void OnMouseClick();
        void OnMouseClick(float delay);
        void OnHoldStarted();
        void OnHoldReleased();
        void ResetChargeVariable();
        void OnUpdate();
        void OnDrawGizmos();
    }
}