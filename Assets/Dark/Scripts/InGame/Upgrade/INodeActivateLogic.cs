namespace InGame.Upgrade
{
    public interface INodeActivateLogic
    {
        void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo);
    }
}