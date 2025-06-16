using System;
using NeatBullets.Core.Scripts.CustomEditor.MinMaxRangeAttribute;
using NeatBullets.Core.Scripts.Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem {
    [Serializable]
    public class WeaponParams : IWeaponParams {
        
        [field: Header("Weapon controls")] 
        [field: SerializeField] public WeaponMode WeaponMode { get; set; }
        [field: SerializeField] public BurstMode BurstMode { get; set; }
        [field: SerializeField] [field: Range(0.02f, 0.3f)] public float BurstRate { get; set; }
        
        [field: SerializeField] [field: Range(0.1f, 1f)] public float FireRate { get; set; }
        [field: SerializeField] [field: Range(1, 30)] public int ProjectilesInOneShot { get; set; }
        
        [field: Header("Coordinate system controls")]
        [field: SerializeField] [field: Range(-120f, 120f)] public float RotationSpeed { get; set; }
        [field: SerializeField] [field: Range(0f, 20f)] public float MoveSpeed { get; set; }
        
        [field: Header("Projectile controls")]
        [field: SerializeField] public NetworkControlMode NetworkControlMode { get; set; }
        [field: SerializeField] public ReadMode ReadMode { get; set; }
        [field: SerializeField] public Vector2 Size { get; set; }
        [field: SerializeField] public bool HasLifespan { get; set; }
        [field: SerializeField] [field: Range(2f, 10f)] public float Lifespan { get; set; }
        [field: SerializeField] public bool HasTrail { get; set; }
        
        [field: SerializeField] [field: MinMaxSlider(0f, 1f)] public Vector2 HueRange { get; set; }
        [field: SerializeField] [field: Range(0f, 1f)] public float Saturation { get; set; }
        [field: SerializeField] [field: Range(0f, 1f)] public float Brightness { get; set; }
        
        [field: Header("NN Pattern controls")] 
        [field: SerializeField] [field: MinMaxSlider(1f, 8f)] public Vector2 SpeedRange { get; set; }
        [field: SerializeField] [field: MinMaxSlider(0.5f, 8f)] public Vector2 ForceRange { get; set; }
        [field: SerializeField] [field: Range(1f, 8f)] public float NNControlDistance { get; set; }
        [field: SerializeField] [field: Range(-1f, 1f)] public float SignX { get; set; }
        [field: SerializeField] [field: Range(-1f, 1f)] public float SignY { get; set; }
        [field: SerializeField] public bool ForwardForce { get; set; }
        
        [field:Header("Initial Flight controls")] 
        [field: SerializeField] [field:Range(0.05f, 0.5f)] public float InitialFlightRadius { get; set; }
        [field: SerializeField] [field:Range(0.5f, 5f)] public float InitialSpeed { get; set; }
        [field: SerializeField] [field:Range(1f, 179.9f)] public float Angle { get; set; }
        
        [field:Header("Reflection controls")] 
        [field: SerializeField] public bool FlipXOnReflect { get; set; }
        [field: SerializeField] public bool FlipYOnReflect { get; set; }
        [field: SerializeField] public ReflectionMode Mode { get; set; }
        
        [field: SerializeField] [field:Range(math.SQRT2, 2f)] public float ReflectiveCircleRadius { get; set; }
        [field: SerializeField] public Vector2 RectDimensions { get; set; }
        [field: SerializeField] [field: Range(5f, 179.9f)] public float MaxPolarAngleDeg { get; set; }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public WeaponParams(IWeaponParams weaponParams) {
            Copy(weaponParams, this);
        }

        /// <summary>
        /// Write data of an original to a copy.
        /// </summary>
        public static void Copy(IWeaponParams Original, IWeaponParams Copy) {
            Copy.WeaponMode = Original.WeaponMode;
            Copy.BurstMode = Original.BurstMode;
            Copy.BurstRate = Original.BurstRate;
            
            Copy.FireRate = Original.FireRate;
            Copy.ProjectilesInOneShot = Original.ProjectilesInOneShot;
            
            Copy.RotationSpeed = Original.RotationSpeed;
            Copy.MoveSpeed = Original.MoveSpeed;
            
            Copy.NetworkControlMode = Original.NetworkControlMode;
            Copy.ReadMode = Original.ReadMode;
            Copy.Size = Original.Size;
            Copy.HasLifespan = Original.HasLifespan;
            Copy.Lifespan = Original.Lifespan;
            Copy.HasTrail = Original.HasTrail;
            
            Copy.HueRange = Original.HueRange;
            Copy.Saturation = Original.Saturation;
            Copy.Brightness = Original.Brightness;
            
            Copy.SpeedRange = Original.SpeedRange;
            Copy.ForceRange = Original.ForceRange;
            Copy.NNControlDistance = Original.NNControlDistance;
            Copy.SignX = Original.SignX;
            Copy.SignY = Original.SignY;
            Copy.ForwardForce = Original.ForwardForce;
            
            Copy.InitialFlightRadius = Original.InitialFlightRadius;
            Copy.InitialSpeed = Original.InitialSpeed;
            Copy.Angle = Original.Angle;
            
            Copy.FlipXOnReflect = Original.FlipXOnReflect;
            Copy.FlipYOnReflect = Original.FlipYOnReflect;
            Copy.Mode = Original.Mode;
            
            Copy.ReflectiveCircleRadius = Original.ReflectiveCircleRadius;
            Copy.RectDimensions = Original.RectDimensions;
            Copy.MaxPolarAngleDeg = Original.MaxPolarAngleDeg;
        }
        
        public WeaponParams() { }
    }

    public enum ReflectionMode {
        CircleReflection,
        RectangleReflection,
        Polar
    }

    public enum NetworkControlMode {
        ForceSum,
        VelocitySum
    }

    public enum WeaponMode {
        MultiShot,
        Burst
    }

    public enum BurstMode {
        Clockwise,
        CounterClockwise,
        Alternate,
        Straight,
        MaxMinAngle,
        Random
    }

    public enum ReadMode {
        Default,
        Rotator
    }
}