using System;
using System.Linq;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame.InGameCinematic
{
    public class CineEndGame : MonoBehaviour
    {
        [SerializeField] private GameObject vfxEndGame;
        [SerializeField] private float delayHideTowers;

        private Transform[] towers;
        private Transform character;
        
        private void Awake()
        {
            // return;
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnLose += OnGameOver;
            
            vfxEndGame.SetActive(false);
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            towers = LevelManager.Instance.Towers.Select((tower) => tower.transform).ToArray();
            character = LevelManager.Instance.Player.transform;
        }
        
        private void OnGameOver()
        {
            vfxEndGame.SetActive(true);
            this.DelayCall(delayHideTowers, () =>
            {
                foreach (var tower in towers)
                    tower.gameObject.SetActive(false);
                
                character.gameObject.SetActive(false);
            });
        }
    }
}