using System;
using NeatBullets.Core.Scripts.WeaponSystem;
using SharpNeat.Phenomes;
using Unity.Mathematics;
using UnityEngine;

namespace NeatBullets.WeaponDemo.Scripts {
    public class ForceArrow : MonoBehaviour {

        // Assigned from UIBox script
        [SerializeField] public WeaponParams WeaponParamsLocal;
        public Transform OriginTransform;
        public float SignX;
        public float SignY;
        public IBlackBox Box;
        public ISignalArray InputArr;
        public ISignalArray OutputArr;
        
        [HideInInspector] public ArrowSpawner ArrowSpawner;

        public SpriteRenderer[] SpriteRenderers;
        private void Awake() {
            SpriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        }
        private void Start() {
            foreach (SpriteRenderer sr in SpriteRenderers) 
                sr.enabled = false;
        }

        public bool InNNControlZone = true;
        public void ActivateBlackBoxCircle() {
            Box.ResetState();

            float maxDistance = WeaponParamsLocal.NNControlDistance * math.SQRT2;

            if (DistanceFromOrigin > maxDistance)
                InNNControlZone = false;

            InputArr[0] = Mathf.Lerp(-1f, 1f, Math.Abs(RelativePos.x) / WeaponParamsLocal.NNControlDistance);
            InputArr[1] = Mathf.Lerp(-1f, 1f, Math.Abs(RelativePos.y) / WeaponParamsLocal.NNControlDistance);
            InputArr[2] = Mathf.Lerp(-1f, 1f, DistanceFromOrigin / maxDistance);

            Box.Activate();
        }

        public void ActivateBlackBoxRect() {
            Box.ResetState();

            float maxX = WeaponParamsLocal.RectDimensions.x * WeaponParamsLocal.NNControlDistance;
            float maxY = WeaponParamsLocal.RectDimensions.y * WeaponParamsLocal.NNControlDistance;
            float maxDistance = maxX > maxY ? maxX : maxY;

            if (Mathf.Abs(RelativePos.x) > maxX || Mathf.Abs(RelativePos.y) > maxY)
                InNNControlZone = false;
            
            InputArr[0] = Mathf.Lerp(-1f, 1f, Math.Abs(RelativePos.x) / maxX);
            InputArr[1] = Mathf.Lerp(-1f, 1f, Math.Abs(RelativePos.y) / maxY);
            InputArr[2] = Mathf.Lerp(-1f, 1f, DistanceFromOrigin / maxDistance);

            Box.Activate();
        }

        public void ActivateBlackBoxPolar() {
            Box.ResetState();

            float maxPhiRad = WeaponParamsLocal.MaxPolarAngleDeg * Mathf.Deg2Rad;
            float NNControlDistance = WeaponParamsLocal.NNControlDistance;

            float x = Math.Abs(DistanceFromOrigin * Mathf.Sin(PhiRad));
            float y = Math.Abs(DistanceFromOrigin * Mathf.Cos(PhiRad));

            float maxX = maxPhiRad * Mathf.Rad2Deg >= 90f ? NNControlDistance : NNControlDistance * Mathf.Sin(maxPhiRad);

            if (PhiRad > maxPhiRad)
                InNNControlZone = false;
            
            InputArr[0] = Mathf.Lerp(-1f, 1f, x / maxX);
            InputArr[1] = Mathf.Lerp(-1f, 1f, y / NNControlDistance);
            InputArr[2] = Mathf.Lerp(-1f, 1f, DistanceFromOrigin / NNControlDistance);

            Box.Activate();
        }

        private float _hue, _force;
        private Vector2 _forceDir;
        public bool ShowForceMagnitude;
        public void ReadDataFromBlackBox() {
            float x = Mathf.Lerp(-1f, 1f, (float)OutputArr[0]) * SignX;
            float y = Mathf.Lerp(-1f, 1f, (float)OutputArr[1]) * SignY;

            switch (WeaponParamsLocal.ReadMode) {
                case ReadMode.Default:
                    _forceDir = OriginTransform.TransformDirection(x, y, 0f);
                    break;

                case ReadMode.Rotator:
                    Vector2 rotatingDir = Vector2.Perpendicular(RelativePosDir).normalized;
                    _forceDir = x * rotatingDir + y * RelativePosDir.normalized;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            transform.up = _forceDir;
            
            _hue = Mathf.Lerp(WeaponParamsLocal.HueRange.x, WeaponParamsLocal.HueRange.y, (float)OutputArr[2]);
            foreach (SpriteRenderer sr in SpriteRenderers) {
                Color spriteColor = Color.HSVToRGB(_hue, WeaponParamsLocal.Saturation, WeaponParamsLocal.Brightness);
                sr.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.3f);
            }
            
            if (ShowForceMagnitude)
                transform.localScale = new Vector2(
                        Mathf.Lerp(ArrowSpawner.ArrowDimensions.x, ArrowSpawner.ArrowDimensions.x * 1.5f, (float)OutputArr[4]),
                        Mathf.Lerp(ArrowSpawner.ArrowDimensions.y, ArrowSpawner.ArrowDimensions.y * 1.5f, (float)OutputArr[4])
                    );
            
        }

        public Vector2 RelativePos;
        public Vector2 RelativePosDir;
        public float DistanceFromOrigin;
        public float PhiRad;
        public void CalcProjectileStats() {
            InNNControlZone = true;
            RelativePosDir = transform.position - OriginTransform.position;
            RelativePos = transform.localPosition;
            DistanceFromOrigin = RelativePos.magnitude;
            PhiRad = Vector2.Angle(OriginTransform.up, RelativePosDir) * Mathf.Deg2Rad;
        }


    }
}
