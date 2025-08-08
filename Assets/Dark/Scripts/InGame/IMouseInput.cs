using System;

namespace InGame
{
    public interface IMouseInput : IDisposable
    {
        void Initialize(InputInGame manager, MoveChargeController chargeController);
        void OnMouseClick();
        void OnHoldStarted();
        void OnHoldReleased();
        void ResetChargeVariable();
        void OnUpdate();
        void OnDrawGizmos();
    }
}