using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace InGame.Effects
{
    public class CameraZoom : IEffect
    {
        public Camera Cam { get; set; }
        public float Duration { get; set; } = 0.2f;
        public Vector3 FocusPosition { get; set; } // Focus point in world space
        
        public IEnumerator DoEffect()
        {
            if (!Cam) yield break;

            var originalOrthoSize = 8f;
            var originalPosition = new Vector3(0f, 0f, -10f);
            var targetOrthoSize = 2.5f;
            
            // Calculate target position and orthographic size
            var worldAfterZoom = Cam.transform.position
                                 + (FocusPosition - Cam.transform.position) * (targetOrthoSize / originalOrthoSize);
            var transition =  FocusPosition - worldAfterZoom;
            transition.z = 0f;
            var targetPosition = Cam.transform.position + transition;

            yield return DOTween.Sequence(this)
                .Append(Cam.DOOrthoSize(targetOrthoSize, Duration).SetEase(Ease.OutQuad))
                .Join(Cam.transform.DOMove(targetPosition, Duration).SetEase(Ease.OutQuad)).WaitForCompletion();
        }

        public void CloneStats(IEffect target)
        {
            var casted = (CameraZoom)target;
            Cam = casted.Cam;
            Duration = casted.Duration;
        }
    }
}