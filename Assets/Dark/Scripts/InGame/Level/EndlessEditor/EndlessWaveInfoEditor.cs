using InGame.EndlessLevel;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace InGame.EndlessEditor
{
    public class EndlessWaveInfoEditor : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inpScaleHp;
        [SerializeField] private TMP_InputField inpScaleDmg;
        [SerializeField] private TMP_InputField inpScaleSpe;
        [SerializeField] private TMP_InputField inpExpRatio;
        [SerializeField] private TMP_InputField inpDarkRatio;
        [SerializeField] private TMP_InputField inpDarkUnitValue;
        [SerializeField] private TMP_InputField inpSigils;
        [SerializeField] private TMP_InputField inpAshes;

        private WaveEndlessInfo cachedWaveInfo;

        private void Awake()
        {
            BindFloatInput(inpScaleHp, value => cachedWaveInfo.scaleHp = value);
            BindFloatInput(inpScaleDmg, value => cachedWaveInfo.scaleDmg = value);
            BindFloatInput(inpScaleSpe, value => cachedWaveInfo.scaleSpe = value);
            BindFloatInput(inpExpRatio, value => cachedWaveInfo.expRatio = value);
            BindFloatInput(inpDarkRatio, value => cachedWaveInfo.darkRatio = value);

            BindIntInput(inpDarkUnitValue, value => cachedWaveInfo.darkUnitValue = value);
            BindIntInput(inpSigils, value => cachedWaveInfo.sigils = value);
            BindIntInput(inpAshes, value => cachedWaveInfo.ashes = value);
        }
        
        public void UpdateValue(WaveEndlessInfo waveInfo)
        {
            cachedWaveInfo = waveInfo;
            if (cachedWaveInfo == null) return;

            inpScaleHp?.SetTextWithoutNotify(cachedWaveInfo.scaleHp.ToString(CultureInfo.InvariantCulture));
            inpScaleDmg?.SetTextWithoutNotify(cachedWaveInfo.scaleDmg.ToString(CultureInfo.InvariantCulture));
            inpScaleSpe?.SetTextWithoutNotify(cachedWaveInfo.scaleSpe.ToString(CultureInfo.InvariantCulture));
            inpExpRatio?.SetTextWithoutNotify(cachedWaveInfo.expRatio.ToString(CultureInfo.InvariantCulture));
            inpDarkRatio?.SetTextWithoutNotify(cachedWaveInfo.darkRatio.ToString(CultureInfo.InvariantCulture));
            inpDarkUnitValue?.SetTextWithoutNotify(cachedWaveInfo.darkUnitValue.ToString(CultureInfo.InvariantCulture));
            inpSigils?.SetTextWithoutNotify(cachedWaveInfo.sigils.ToString(CultureInfo.InvariantCulture));
            inpAshes?.SetTextWithoutNotify(cachedWaveInfo.ashes.ToString(CultureInfo.InvariantCulture));
        }

        private void BindFloatInput(TMP_InputField input, System.Action<float> onParsed)
        {
            if (!input) return;
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(rawValue =>
            {
                if (cachedWaveInfo == null) return;
                if (!float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) return;
                onParsed?.Invoke(value);
            });
        }

        private void BindIntInput(TMP_InputField input, System.Action<int> onParsed)
        {
            if (!input) return;
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(rawValue =>
            {
                if (cachedWaveInfo == null) return;
                if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return;
                onParsed?.Invoke(value);
            });
        }
    }
}