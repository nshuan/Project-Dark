using System;

namespace InGame
{
    public interface IMouseInput : IDisposable
    {
        InputInGame InputManager { get; set; }
        void OnMouseClick();
        void OnUpdate();
        void OnDrawGizmos();
    }
}