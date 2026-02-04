using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace InGame.BossConfig
{
    [CreateAssetMenu(menuName = "InGame/Boss/Boss Tarnished", fileName = "BossTheTarnished")]
    public class BossTarnishedConfig : BossBehaviourConfig
    {
        public string stringHpThreshold;
        public string stringTargetIdOrder;
        public string stringStartDistance;

        public List<float> GetHpThreshold()
        {
            var result = new List<float>();
            if (string.IsNullOrEmpty(stringHpThreshold)) return result;
            var split = stringHpThreshold.Split(',');
            foreach (var s in split)
            {
                if (float.TryParse(s, NumberStyles.Float, GameConst.FloatCulture, out var threshold))
                {
                    result.Add(threshold);
                }
            }

            return result;
        }

        public List<int> GetTargetIdOrder()
        {
            var result = new List<int>();
            if (string.IsNullOrEmpty(stringTargetIdOrder)) return result;
            var split = stringTargetIdOrder.Split(',');
            foreach (var s in split)
            {
                if (int.TryParse(s, out var targetId))
                    result.Add(targetId);
            }

            return result;
        }

        public List<float> GetStartDistance()
        {
            var result = new List<float>();
            if (string.IsNullOrEmpty(stringStartDistance)) return result;
            var split = stringStartDistance.Split(',');
            foreach (var s in split)
            {
                if (float.TryParse(s, NumberStyles.Float, GameConst.FloatCulture, out var threshold))
                {
                    result.Add(threshold);
                }
            }

            return result;
        }
    }
}