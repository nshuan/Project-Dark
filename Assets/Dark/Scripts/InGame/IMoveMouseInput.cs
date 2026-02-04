using UnityEngine;

namespace InGame
{
    public interface IMoveTowerMouseInput : IMoveMouseInput
    {
        bool BlockHover { get; set; }
        void OnMouseClick();
        void OnActivated();
        void OnDeactivated();
        void OnDrawGizmos();
    }

    public interface IMoveMouseInput
    {
        bool CanMove { get; }
        void OnUpdate(Vector2 worldMousePosition);
        void Deactivate();
    }
}