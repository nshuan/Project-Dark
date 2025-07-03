using UnityEngine;

namespace InGame.Upgrade.Nodes
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Node/Player Damage", fileName = "NodePlayerDamage")]
    public class NodePlayerDamage : UpgradeNodeConfig
    {
        public int value;
        public bool isMultiply;
        
        public override void ActivateNode(ref UpgradeBonusInfo bonusInfo)
        {
            if (isMultiply)
                bonusInfo.dameMultiply += value;
            else
                bonusInfo.damePlus += value;
        }
    }
}