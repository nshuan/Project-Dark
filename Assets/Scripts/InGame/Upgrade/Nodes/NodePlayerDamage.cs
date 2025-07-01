using InGame.ConfigManager;
using UnityEngine;

namespace InGame.Upgrade.Nodes
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Node/Player Damage", fileName = "NodePlayerDamage")]
    public class NodePlayerDamage : UpgradeNodeConfig
    {
        public int value;
        public bool isMultiply;
        
        public override void ActivateNode()
        {
            var baseValue = ConfigManifest.Instance.PlayerConfig.damage;
            var addValue = isMultiply ? baseValue * value / 100 : value;
            ConfigManifest.Instance.PlayerConfig.damage += addValue;     
        }
    }
}