using System;
using System.Collections.Generic;
using Dark.Tools.GoogleSheetTool;
using Data;

namespace InGame.Upgrade.NodeLogicsV2
{
    [Serializable]
    [ConfigNodeLogicTypeV2(NodeBonusTypeV2.UnlockNormalAttackPiercing)]
    public class NodeV2UnlockNormalAttackPiercing : INodeActivateLogicV2, INodeLogicGeneratorV2
    {
        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (bonusInfo == null) return;
            bonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing = true;
            
            // Class knight khi unlock normal piercing thì x1.5 lên range và stagger
            if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight)
            {
                bonusInfo.bonusNormalAttack.bonusNormalAttackRange.mul += 0.5f;
                bonusInfo.bonusBase.bonusStagger.mul += 0.5f;
            }
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