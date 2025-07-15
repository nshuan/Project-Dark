namespace InGame.UI.CombatSkills
{
    public class UIAttackNormalIcon : UIInGameSkillIcon
    {
        private void Start()
        {
            CombatActions.OnAttackNormal += OnSkillUsed;
        }

        private void OnDestroy()
        {
            CombatActions.OnAttackNormal -= OnSkillUsed;
        }
    }
}