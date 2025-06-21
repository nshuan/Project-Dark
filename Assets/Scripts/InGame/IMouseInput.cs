using System;

namespace InGame
{
    public interface IMouseInput : IDisposable
    {
        void OnMouseClick();
        void OnUpdate();
        void OnDrawGizmos();
    }
}