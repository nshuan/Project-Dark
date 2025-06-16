using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEngine;

namespace NeatBullets.Core.Scripts.Interfaces {
    
    public interface IWeaponParams {
        WeaponMode WeaponMode { get; set; }
        BurstMode BurstMode { get; set; }
        float BurstRate { get; set; }
        float FireRate { get; set; }
        int ProjectilesInOneShot { get; set; }
        
        float RotationSpeed { get; set; }
        float MoveSpeed { get; set; }
        
        NetworkControlMode NetworkControlMode { get; set; }
        ReadMode ReadMode { get; set; }
        Vector2 Size { get; set; }
        bool HasLifespan { get; set; } 
        float Lifespan { get; set; }
        bool HasTrail { get; set; }
        
        Vector2 HueRange { get; set; }
        float Saturation { get; set; }
        float Brightness { get; set; }
        
        Vector2 SpeedRange { get; set; }
        Vector2 ForceRange { get; set; }
        float NNControlDistance { get; set; }
        float SignX { get; set; }
        float SignY { get; set; }
        bool ForwardForce { get; set; }
        
        float InitialFlightRadius { get; set; }
        float InitialSpeed { get; set; }
        float Angle { get; set; }
        
        bool FlipXOnReflect { get; set; }
        bool FlipYOnReflect { get; set; }
        ReflectionMode Mode { get; set; }
        
        float ReflectiveCircleRadius { get; set; }
        Vector2 RectDimensions { get; set; }
        float MaxPolarAngleDeg { get; set; }
    }
}
