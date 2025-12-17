using UnityEngine;

namespace InGame
{
    public interface IMoveTowerMouseInput : IMoveMouseInput
    {
        void OnMouseClick(bool isLongTele);
        void OnActivated();
        void OnDeactivated();
    }

    public interface IMoveMouseInput
    {
        bool CanMove { get; }
        void OnUpdate(Vector2 worldMousePosition);
        void Deactivate();
    }
}