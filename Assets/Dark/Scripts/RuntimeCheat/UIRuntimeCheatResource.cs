using System;
using Economic;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatResource : MonoBehaviour
    {
        public TMP_InputField inpSetVestige;
        public TMP_InputField inpSetSigil;
        public TMP_InputField inpSetEchoes;

        private void OnEnable()
        {
            inpSetVestige.text = WealthManager.Instance.Dark.ToString();
            inpSetSigil.text = WealthManager.Instance.BossPoint.ToString();
            inpSetEchoes.text = WealthManager.Instance.LevelPoint.ToString();
        }

        public void Save()
        {
            if (int.TryParse(inpSetVestige.text, out var valueInt))
            {
                WealthManager.Instance.AddDark(valueInt - WealthManager.Instance.Dark);
            }

            if (int.TryParse(inpSetSigil.text, out valueInt))
            {
                WealthManager.Instance.AddBossPoint(valueInt - WealthManager.Instance.BossPoint);
            }

            if (int.TryParse(inpSetEchoes.text, out valueInt))
            {
                WealthManager.Instance.AddLevelPoint(valueInt - WealthManager.Instance.LevelPoint);
            }
        }
    }
}