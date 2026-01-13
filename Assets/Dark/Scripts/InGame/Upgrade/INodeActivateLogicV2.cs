namespace InGame.Upgrade
{
    public interface INodeActivateLogicV2
    {
        void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo);
        (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo);
        string GetDisplayValue(int level);
        int MaxLevel { get; }
    }

    public interface INodeDynamicBonusValueV2
    {
        bool IsDynamic { get; }
        void OverrideBonusValue(int groupUnlockOrder);
    }
}