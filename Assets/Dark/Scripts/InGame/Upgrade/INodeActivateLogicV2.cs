namespace InGame.Upgrade
{
    public interface INodeActivateLogicV2
    {
        void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo);
        (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo);
        string GetDisplayValue(int level);
        int MaxLevel { get; }
    }
}