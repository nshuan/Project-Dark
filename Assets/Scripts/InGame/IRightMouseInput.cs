namespace InGame
{
    public interface IRightMouseInput
    {
        void OnMouseClick(bool isLongTele);
        void OnActivated();
        void OnDeactivated();
        void OnUpdate();
    }
}