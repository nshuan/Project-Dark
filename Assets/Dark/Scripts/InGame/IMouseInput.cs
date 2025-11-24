using System;

namespace InGame
{
    public interface IMouseInput : IDisposable, IMoveMouseInput
    {
        void Initialize(InputInGame manager, MoveChargeController chargeController);
        void OnMouseClick();
        void OnHoldStarted();
        void OnHoldReleased();
        void ResetChargeVariable();
        bool CanCharge { get; }
        void OnDrawGizmos();
    }
}