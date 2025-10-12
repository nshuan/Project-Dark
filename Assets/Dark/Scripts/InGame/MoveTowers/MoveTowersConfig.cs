using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Move Towers Config", fileName = "MoveTowersConfig")]
    public class MoveTowersConfig : SerializedScriptableObject
    {
        public int id;
        public float delayMove;
        public float cooldown;
        public int damage;
        public float size;
        public float stagger;
        public int maxHitEachTrigger = 5;
        [NonSerialized, OdinSerialize] public IMoveTowersLogic moveLogic;
        
        private IMoveTowersLogic moveFuseLogic;

        public IMoveTowersLogic MoveFuseLogic
        {
            get
            {
                if (moveFuseLogic == null)
                {
                    if (moveLogic is MoveDashToTower dashLogic)
                        moveFuseLogic = new MoveDashFuseToTower(dashLogic);
                    else if (moveLogic is MoveFlashToTower flashLogic)
                        moveFuseLogic = new MoveFlashFuseToTower(flashLogic);
                }
                
                return moveFuseLogic;
            }
        }
    }
}