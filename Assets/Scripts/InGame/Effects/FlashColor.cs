using System.Collections;
using UnityEngine;

namespace InGame.Effects
{
    public class FlashColor : IEffect
    {
        public float Duration { get; set; }
        public float FlashDuration { get; set; }
        public SpriteRenderer SpriteRendererTarget { get; set; }
        public Color Color { get; set; }
        
        public IEnumerator DoEffect()
        {
            SpriteRendererTarget.color = Color;
            yield return new WaitForSeconds(FlashDuration);
            SpriteRendererTarget.color = Color.white;
        }

        public void CloneStats(IEffect target)
        {
            var casted = (FlashColor)target;
            FlashDuration = casted.FlashDuration;
            SpriteRendererTarget = casted.SpriteRendererTarget;
            Color = casted.Color;
        }
    }
}