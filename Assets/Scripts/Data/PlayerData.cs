using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public int level;
        public List<int> unlockedUpgradeNodes;

        public PlayerData()
        {
            level = 1;
            unlockedUpgradeNodes = new List<int>();
        }
    }
}