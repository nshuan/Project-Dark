using System;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveToTower : IRightMouseInput
    {
        private const float HoverRadius = 2f;
        
        private PlayerCharacter Character { get; set; }
        private MoveTowersConfig ShortConfig { get; set; }
        private MoveTowersConfig LongConfig { get; set; }
        private TowerEntity[] Towers { get; set; }
        private int CurrentTowerIndex { get; set; }
        private Camera Cam { get; set; }
        private Func<float, Action, bool> DelayCallFunction { get; set; }
        public bool IsActivate { get; set; } 

        private bool CanMoveLong { get; set; }
        public bool CanMove { get; set; }
        private bool CanCountdown { get; set; }
        protected float Cooldown { get; set; }
        protected float cdCounter;
        
        private Vector2 worldMousePosition;
        private bool hovering;
        private Vector2 hoveringCenter;
        private Action<TowerEntity> actionTowerChanged;
        private int selectingTower = -1;

        public MoveToTower(Camera cam, PlayerCharacter player, MoveTowersConfig shortConfig, MoveTowersConfig longConfig, TowerEntity[] towers, int currentTowerIndex, Func<float, Action, bool> delayCallFunction)
        {
            Cam = cam;
            Character = player;
            ShortConfig = shortConfig;
            LongConfig = longConfig;
            Towers = towers;
            CurrentTowerIndex = currentTowerIndex;
            DelayCallFunction = delayCallFunction;
            actionTowerChanged = OnTowerChanged;
            CanMove = false;
            CanCountdown = true;
            CanMoveLong = LevelUtility.BonusInfo.unlockedLongMoveToTower;

            LevelManager.Instance.OnChangeTower += actionTowerChanged;
        }

        ~MoveToTower()
        {
            actionTowerChanged = null;
        }

        public void OnMouseClick(bool isLongTele)
        {
            if (!CanMove) return;
            if (isLongTele && !CanMoveLong) return;
            if (selectingTower > -1)
            {
                CanMove = false;
                CanCountdown = false;
                if (isLongTele)
                {
                    Character.StartCoroutine(LongConfig.moveLogic.IEMove(
                        Character, 
                        Character.transform.position, 
                        Towers[selectingTower].transform.position + Towers[selectingTower].standOffset,
                        () =>
                        {
                            LevelManager.Instance.TeleportTower(selectingTower);
                            Cooldown = LongConfig.cooldown;
                            cdCounter = Cooldown;
                            CanCountdown = true;
                            CurrentTowerIndex = selectingTower;
                        }));
                }
                else
                {
                    Character.StartCoroutine(ShortConfig.moveLogic.IEMove(
                        Character, 
                        Character.transform.position, 
                        Towers[selectingTower].transform.position + Towers[selectingTower].standOffset,
                        () =>
                        {
                            LevelManager.Instance.TeleportTower(selectingTower);
                            Cooldown = ShortConfig.cooldown;
                            cdCounter = Cooldown;
                            CanCountdown = true;
                            CurrentTowerIndex = selectingTower;
                        }));
                }
            }
        }

        public void OnActivated(bool isLongTele)
        {
            if (Towers == null) return;
            if (!CanMove) return;
            if (isLongTele && !CanMoveLong) return;
            IsActivate = true;
            foreach (var tower in Towers)
            {
                if (tower.IsDestroyed) continue;
                if (tower.Id == CurrentTowerIndex) continue;
                tower.Highlight(true);
            }
        }

        public void OnDeactivated()
        {
            if (Towers == null) return;
            IsActivate = false;
            foreach (var tower in Towers)
            {
                tower.Highlight(false);
                tower.Hover(false);
            }
        }

        public void OnUpdate()
        {
            if (IsActivate)
            {
                worldMousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
                if (!hovering)
                {
                    foreach (var tower in Towers)
                    {
                        if (Vector2.Distance(tower.transform.position, worldMousePosition) < HoverRadius)
                        {
                            hovering = true;
                            hoveringCenter = tower.transform.position;
                            selectingTower = tower.Id;
                            tower.Hover(true);
                            break;
                        }
                    }
                }

                if (hovering)
                {
                    if (Vector2.Distance(worldMousePosition, hoveringCenter) >= HoverRadius)
                    {
                        hovering = false;
                        Towers[selectingTower].Hover(false);
                        selectingTower = -1;
                    }
                }
            }

            if (!CanMove)
            {
                if (CanCountdown)
                {
                    cdCounter -= Time.deltaTime;
                    if (cdCounter <= 0)
                        CanMove = true;
                }
            }
        }

        private void OnTowerChanged(TowerEntity tower)
        {
            CurrentTowerIndex = tower.Id;
        }
    }
}