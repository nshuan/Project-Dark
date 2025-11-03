using InGame.ProjectileCustomPath;
using UnityEngine;
using UnityEngine.Serialization;

public class Demo : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;

    [SerializeField] private float shootRate;
    [SerializeField] private float projectileMaxMoveSpeed;
    [SerializeField] private float projectileMaxHeight;

    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
    
    private float shootTimer;

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            shootTimer += shootRate;
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<TargetedProjectile>();
            projectile.InitializeProjectile(target.position, projectileMaxMoveSpeed, projectileMaxHeight);
            projectile.InitializeAnimationCurve(trajectoryAnimationCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);
        }
    }
}
