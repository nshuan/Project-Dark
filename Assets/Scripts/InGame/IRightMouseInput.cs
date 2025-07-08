namespace InGame
{
    public interface IRightMouseInput
    {
        void OnMouseClick(bool isLongTele);
        void OnActivated(bool isLongTele);
        void OnDeactivated();
        void OnUpdate();
    }
}