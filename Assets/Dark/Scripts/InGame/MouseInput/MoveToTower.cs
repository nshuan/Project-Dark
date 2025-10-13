using System;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveToTower : IMoveTowerMouseInput
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
        public bool CanMove { get; private set; }
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
            Towers = towers;
            CurrentTowerIndex = currentTowerIndex;
            DelayCallFunction = delayCallFunction;
            actionTowerChanged = OnTowerChanged;
            CanMove = false;
            CanCountdown = true;
            
            // bỏ cơ chế move bằng chuột phải, thay bằng cơ chế kết hợp 2 loại move
            // CanMoveLong = longConfig != null;
            CanMoveLong = false;
            ShortConfig = shortConfig;
            LongConfig = longConfig;

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

                Action callbackComplete = () =>
                {
                    LevelManager.Instance.TeleportTower(selectingTower);
                    Cooldown = GetCooldown(ShortConfig);
                    cdCounter = Cooldown;
                    CanCountdown = true;
                    CurrentTowerIndex = selectingTower;
                    CombatActions.OnMoveTower?.Invoke(Cooldown);
                };
                
                ShortConfig.moveLogic.SetStats(
                    GetDamage(ShortConfig),
                    ShortConfig.stagger,
                    ShortConfig.maxHitEachTrigger,
                    GetSize(ShortConfig));
                if (!LongConfig)
                {
                    Character.StartCoroutine(ShortConfig.moveLogic.IEMove(
                        Character, 
                        Towers[CurrentTowerIndex], 
                        Towers[selectingTower],
                        callbackComplete
                    ));
                }
                else
                {
                    ShortConfig.moveLogic.SetStatsFuse(
                        GetDamage(LongConfig),
                        LongConfig.stagger,
                        LongConfig.maxHitEachTrigger,
                        GetSize(LongConfig));
                    
                    Character.StartCoroutine(ShortConfig.MoveFuseLogic.IEMove(
                        Character, 
                        Towers[CurrentTowerIndex], 
                        Towers[selectingTower],
                        callbackComplete
                    ));
                }
                
                foreach (var tower in Towers)
                {
                    tower.Hover(false);
                }
            }
        }

        public void OnActivated()
        {
            if (Towers == null) return;
            if (!CanMove) return;
            IsActivate = true;
        }

        public void OnDeactivated()
        {
            if (Towers == null) return;
            IsActivate = false;
            foreach (var tower in Towers)
            {
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
                        if (tower.Id == CurrentTowerIndex) continue;
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

        private float GetCooldown(MoveTowersConfig config)
        {
            var cooldown = config.cooldown;
            if (config.moveLogic is MoveDashToTower) cooldown = LevelUtility.GetDashCooldown(config.cooldown);
            else if (config.moveLogic is MoveFlashToTower) cooldown = LevelUtility.GetFlashCooldown(config.cooldown);
            else if (config.moveLogic is MoveTeleToTower) cooldown = LevelUtility.GetTeleCooldown(config.cooldown);
            return cooldown;
        }

        private float GetSize(MoveTowersConfig config)
        {
            var size = config.size;
            if (config.moveLogic is MoveDashToTower) size = LevelUtility.GetDashSize(config.size);
            else if (config.moveLogic is MoveFlashToTower) size = LevelUtility.GetFlashSize(config.size);
            return size;
        }

        private int GetDamage(MoveTowersConfig config)
        {
            var damage = config.damage;
            if (config.moveLogic is MoveDashToTower) damage = LevelUtility.GetDashDamage(config.damage);
            else if (config.moveLogic is MoveFlashToTower) damage = LevelUtility.GetFlashDamage(config.damage);
            return damage;
        }
    }
}