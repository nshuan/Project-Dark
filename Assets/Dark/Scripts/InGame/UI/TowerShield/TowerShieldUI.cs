using UnityEngine;

namespace InGame.UI
{
    public class TowerShieldUI : MonoBehaviour
    {
        [SerializeField] private TowerEntity tower;
        [SerializeField] private SpriteRenderer shieldFill;
        [SerializeField] private SpriteRenderer shieldGlow;

        private Vector3 tempShieldScale;
        
        private void Start()
        {
            tower.OnHitShield += OnHitShield;
            tower.shield.OnRegenerate += OnShieldChanged;
        }

        private void OnHitShield(int damage, DamageType dmgType)
        {
            OnShieldChanged(damage);
        }

        private void OnShieldChanged(int value)
        {
            tempShieldScale.x = shieldFill.transform.localScale.x;
            tempShieldScale.y = shieldFill.transform.localScale.y;
            tempShieldScale.z = shieldFill.transform.localScale.z;
            tempShieldScale.y = Mathf.Clamp((float)tower.shield.CurrentShield / tower.shield.MaxShield, 0f, 1f);
            shieldFill.transform.localScale = tempShieldScale; 
        }
    }
}