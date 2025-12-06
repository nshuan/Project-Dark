using System;

namespace InGame.Upgrade
{
    public interface INodeActivateLogic
    {
        void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo);
        (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo);
        string GetDisplayValue(int level);
        int MaxLevel { get; }
    }
}