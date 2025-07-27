using UnityEngine;

namespace InGame
{
    /// <summary>
    /// LOGIC ONLY
    /// 
    /// Class inherited from this interface must not use any temporary variable
    /// because an only instance of the class is used in every projectile
    /// </summary>
    public interface IProjectileActivate
    {
        void DoAction(ProjectileEntity parentProjectile, Vector2 direction);
    }
}