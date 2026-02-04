using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Passive Burn Effect", fileName = "PassiveBurnConfig")]
    public class PassiveBurnConfig : PassiveConfig
    {
        public float burnInterval = 1f;

        public override float[] GetAdditionalParams()
        {
            return new float[] { burnInterval };
        }
    }
}