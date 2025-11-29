namespace InGame.Upgrade
{
    public interface INodeActivateLogic
    {
        void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo);
        string GetDisplayValue(int level);
        (string, string) GetBeforeAfterValue(int level);
        int MaxLevel { get; }
    }
}