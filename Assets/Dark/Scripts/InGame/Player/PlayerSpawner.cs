using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class PlayerSpawner : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private Dictionary<CharacterClass.CharacterClass, PlayerCharacter> characterMap;

        public PlayerCharacter SpawnCharacter(CharacterClass.CharacterClass characterClass)
        {
            return Instantiate(characterMap[characterClass]);
        }
    }
}