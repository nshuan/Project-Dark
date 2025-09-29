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
        void OnUpdate();
    }
}