using Core;
using Data;

namespace InGame.CharacterClass
{
    public class ClassManager : Singleton<ClassManager>
    {
        public CharacterClass CurrentClass { get; set; }

        public ClassManager()
        {
            CurrentClass = (CharacterClass)PlayerDataManager.Instance.Data.characterClass;
        }
    }

    public enum CharacterClass
    {
        None,
        Archer,
        Knight
    }
}