using System.Collections.Generic;
using Core;

namespace InGame
{
    public class GateManager : MonoSingleton<GateManager>
    {
        private List<GateEntity> listGateInLevel;
        public List<GateEntity> ListGateInLevel => listGateInLevel;

        public void AddGate(GateEntity gate)
        {
            listGateInLevel ??= new List<GateEntity>();
            if (listGateInLevel.Contains(gate)) return;
            listGateInLevel.Add(gate);
        }
    }
}