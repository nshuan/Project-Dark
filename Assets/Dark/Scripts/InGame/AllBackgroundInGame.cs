using System.Collections.Generic;
using Core;

namespace InGame
{
    public class AllBackgroundInGame : MonoSingleton<AllBackgroundInGame>
    {
        public List<BackgroundInGame> allBackgroundInGame;

        public BackgroundInGame CurrentBackground { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            allBackgroundInGame ??= new List<BackgroundInGame>();
        }

        public void SetCurrentBackground(int index)
        {
            for (var i = 0; i < allBackgroundInGame.Count; i++)
            {
                if (i == index)
                {
                    CurrentBackground = allBackgroundInGame[i];
                    break;
                }
            }
        }
    }
}