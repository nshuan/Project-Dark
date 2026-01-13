using System;
using System.Collections.Generic;
using Dark.Tools.GoogleSheetTool;

namespace InGame.Upgrade.NodeLogicsV2
{
    [Serializable]
    [ConfigNodeLogicTypeV2(NodeBonusTypeV2.UnlockPassiveChargeThunder)]
    public class NodeV2UnlockPassiveChargeThunder : INodeActivateLogicV2, INodeLogicGeneratorV2
    {
        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (bonusInfo == null) return;
            bonusInfo.bonusUnlockSkill.unlockPassiveChargeThunder = true;
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            return ("", "");
        }

        public string GetDisplayValue(int level)
        {
            return "";
        }

        public int MaxLevel => 1;
        public INodeActivateLogicV2 Generate(string subType, List<string> listValue, bool mul)
        {
            return this;
        }
    }
}

