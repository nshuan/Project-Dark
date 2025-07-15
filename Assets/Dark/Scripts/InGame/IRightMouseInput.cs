namespace InGame
{
    public interface IRightMouseInput
    {
        bool CanMove { get; }
        void OnMouseClick(bool isLongTele);
        void OnActivated();
        void OnDeactivated();
        void OnUpdate();
    }
}