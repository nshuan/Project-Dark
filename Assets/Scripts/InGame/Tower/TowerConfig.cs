using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Tower/Tower Config", fileName = "TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        public int hp;
    }
}