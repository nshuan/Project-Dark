using System;
using System.Collections.Generic;
using InGame;
using InGame.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestVfxInGame
{
    public class CheatButtonsInGame : MonoBehaviour
    {
        [SerializeField] private TestUpgradeBonus testUpgradeBonus;
        
        [Space]
        [SerializeField] private Button btnUnlockExplosion;
        [SerializeField] private Button btnUnlockLightning;
        [SerializeField] private Button btnUnlockBurning;
        [SerializeField] private Button btnUnlockThunder;

        private UpgradeBonusInfo tempBonusInfo;
        
        private void Awake()
        {
            btnUnlockExplosion.onClick.RemoveAllListeners();
            btnUnlockLightning.onClick.RemoveAllListeners();
            btnUnlockBurning.onClick.RemoveAllListeners();
            btnUnlockThunder.onClick.RemoveAllListeners();
            btnUnlockExplosion.onClick.AddListener(OnButtonExplosionClicked);
            btnUnlockLightning.onClick.AddListener(OnButtonLightningClicked);
            btnUnlockBurning.onClick.AddListener(OnButtonBurningClicked);
            btnUnlockThunder.onClick.AddListener(OnButtonThunderClicked);
        }

        private void Start()
        {
            btnUnlockExplosion.GetComponentInChildren<TextMeshProUGUI>().SetText(
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.ContainsKey(PassiveTriggerType.DameByNormalAttack) ||
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack] == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Explosion)
                    ? "Unlock Explosion" : "Lock Explosion");
            btnUnlockLightning.GetComponentInChildren<TextMeshProUGUI>().SetText(
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.ContainsKey(PassiveTriggerType.DameByNormalAttack) ||
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack] == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Lightning)
                    ? "Unlock Lightning" : "Lock Lightning");
            btnUnlockBurning.GetComponentInChildren<TextMeshProUGUI>().SetText(
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.ContainsKey(PassiveTriggerType.DameByNormalAttack) ||
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack] == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Burning)
                    ? "Unlock Burning" : "Lock Burning");
            btnUnlockThunder.GetComponentInChildren<TextMeshProUGUI>().SetText(
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.ContainsKey(PassiveTriggerType.DameByNormalAttack) ||
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack] == null ||
                !testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Thunder)
                    ? "Unlock Thunder" : "Lock Thunder");
        }

        private void OnButtonExplosionClicked()
        {
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType ??=
                new Dictionary<PassiveTriggerType, List<PassiveType>>();
            
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.TryAdd(PassiveTriggerType.DameByNormalAttack, new List<PassiveType>());
            if (!testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Explosion))
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Explosion);
                btnUnlockExplosion.GetComponentInChildren<TextMeshProUGUI>().SetText("Lock Explosion");
            }
            else
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Remove(PassiveType.Explosion);
                btnUnlockExplosion.GetComponentInChildren<TextMeshProUGUI>().SetText("Unlock Explosion");
            }
            
            UpgradeManager.Instance.ActivateTree(ref tempBonusInfo);
        }

        private void OnButtonLightningClicked()
        {
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType ??=
                new Dictionary<PassiveTriggerType, List<PassiveType>>();
            
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.TryAdd(PassiveTriggerType.DameByNormalAttack, new List<PassiveType>());
            if (!testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Lightning))
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Lightning);
                btnUnlockLightning.GetComponentInChildren<TextMeshProUGUI>().SetText("Lock Lightning");
            }
            else
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Remove(PassiveType.Lightning);
                btnUnlockLightning.GetComponentInChildren<TextMeshProUGUI>().SetText("Unlock Lightning");
            }
            
            UpgradeManager.Instance.ActivateTree(ref tempBonusInfo);
        }

        private void OnButtonBurningClicked()
        {
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType ??=
                new Dictionary<PassiveTriggerType, List<PassiveType>>();
            
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.TryAdd(PassiveTriggerType.DameByNormalAttack, new List<PassiveType>());
            if (!testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Burning))
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Burning);
                btnUnlockBurning.GetComponentInChildren<TextMeshProUGUI>().SetText("Lock Burning");
            }
            else
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Remove(PassiveType.Burning);
                btnUnlockBurning.GetComponentInChildren<TextMeshProUGUI>().SetText("Unlock Burning");
            }
            
            UpgradeManager.Instance.ActivateTree(ref tempBonusInfo);
        }

        private void OnButtonThunderClicked()
        {
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType ??=
                new Dictionary<PassiveTriggerType, List<PassiveType>>();
            
            testUpgradeBonus.testBonusInfo.passiveMapByTriggerType.TryAdd(PassiveTriggerType.DameByNormalAttack, new List<PassiveType>());
            if (!testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Contains(PassiveType.Thunder))
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Add(PassiveType.Thunder);
                btnUnlockThunder.GetComponentInChildren<TextMeshProUGUI>().SetText("Lock Thunder");
            }
            else
            {
                testUpgradeBonus.testBonusInfo.passiveMapByTriggerType[PassiveTriggerType.DameByNormalAttack].Remove(PassiveType.Thunder);
                btnUnlockThunder.GetComponentInChildren<TextMeshProUGUI>().SetText("Unlock Thunder");
            }
            
            UpgradeManager.Instance.ActivateTree(ref tempBonusInfo);
        }
    }
}