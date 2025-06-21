using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Behaviour", fileName = "PlayerSKillBehaviour")]   
    public class PlayerSkillBehaviour : ScriptableObject
    {
        public ShotCursorType cursorType;
    }
}