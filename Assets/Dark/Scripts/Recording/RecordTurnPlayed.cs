using System;
using Data;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Recording
{
    public class RecordTurnPlayed : MonoBehaviour
    {
        private void Awake()
        {
            LevelManager.Instance.OnWin += OnCompletedTurn;
            LevelManager.Instance.OnLose += OnCompletedTurn;
        }

        private void OnCompletedTurn()
        {
            var data = PlayerDataManager.Instance.Data;
            data.passedDay += 1;
            PlayerDataManager.Instance.Save(data);
        }
    }
}