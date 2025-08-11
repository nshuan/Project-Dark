using System.Linq;

namespace InGame
{
    public class WinLoseManager
    {
        public void CheckLose(LevelManager levelManager)
        {
            if (levelManager.Towers.Any((tower) => tower.IsDestroyed))
                levelManager.LoseLevel();
        }

        public void CheckWin(LevelManager levelManager)
        {
            if (levelManager.Level.waveInfo.All((wave) => wave.WaveEndedCompletely))
                levelManager.WinLevel();
        }
    }
}