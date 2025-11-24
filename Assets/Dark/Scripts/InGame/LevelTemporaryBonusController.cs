using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Mỗi loại temporary bonus sẽ chạy độc lập, và không được cộng dồn
    /// Nghĩa là ví dụ đang bonus dame nhờ kill enemy, thì kill enemy tiếp sẽ không tác động gì thêm lên bonus
    /// </summary>
    public class LevelTemporaryBonusController : MonoBehaviour
    {
        private Coroutine coroutineTempDamageOnKill;
        private Coroutine coroutineTempDamageOnMove;
        private Coroutine coroutineTempAtkSpeOnKill;
        private Coroutine coroutineTempAtkSpeOnMove;
        
        private void Awake()
        {
            CombatActions.OnOneEnemyDead += OnEnemyKilled;
            CombatActions.OnMoveTower += OnMoveTower;
        }

        private void OnEnemyKilled(EnemyEntity enemy)
        {
            if (LevelTemporaryUtility.activatedTemporaryDamageOnKill == false && 
                LevelUtility.BonusInfo.tempDamageBonusOnKill != null && 
                LevelUtility.BonusInfo.tempDamageBonusOnKill.bonusDuration > 0)
            {
                LevelTemporaryUtility.activatedTemporaryDamageOnKill = true;
                if (coroutineTempDamageOnKill != null) StopCoroutine(coroutineTempDamageOnKill);
                coroutineTempDamageOnKill = StartCoroutine(IETemporaryBonus(
                    LevelUtility.BonusInfo.tempDamageBonusOnKill.bonusDuration,
                    () =>
                    {
                        LevelTemporaryUtility.activatedTemporaryDamageOnKill = false;
                    }));
            }
            
            if (LevelTemporaryUtility.activatedTemporaryAtkSpeOnKill == false && 
                LevelUtility.BonusInfo.tempAtkSpeBonusOnKill != null && 
                LevelUtility.BonusInfo.tempAtkSpeBonusOnKill.bonusDuration > 0)
            {
                LevelTemporaryUtility.activatedTemporaryAtkSpeOnKill = true;
                if (coroutineTempAtkSpeOnKill != null) StopCoroutine(coroutineTempAtkSpeOnKill);
                coroutineTempAtkSpeOnKill = StartCoroutine(IETemporaryBonus(
                    LevelUtility.BonusInfo.tempAtkSpeBonusOnKill.bonusDuration,
                    () =>
                    {
                        LevelTemporaryUtility.activatedTemporaryAtkSpeOnKill = false;
                    }));
            }
        }

        private void OnMoveTower(float cooldown)
        {
            if (LevelTemporaryUtility.activatedTemporaryDamageOnMove == false && 
                LevelUtility.BonusInfo.tempDamageBonusOnMove != null && 
                LevelUtility.BonusInfo.tempDamageBonusOnMove.bonusDuration > 0)
            {
                LevelTemporaryUtility.activatedTemporaryDamageOnMove = true;
                if (coroutineTempDamageOnMove != null) StopCoroutine(coroutineTempDamageOnMove);
                coroutineTempDamageOnMove = StartCoroutine(IETemporaryBonus(
                    LevelUtility.BonusInfo.tempDamageBonusOnMove.bonusDuration,
                    () =>
                    {
                        LevelTemporaryUtility.activatedTemporaryDamageOnMove = false;
                    }));
            }
            
            if (LevelTemporaryUtility.activatedTemporaryAtkSpeOnMove == false && 
                LevelUtility.BonusInfo.tempAtkSpeBonusOnMove != null && 
                LevelUtility.BonusInfo.tempAtkSpeBonusOnMove.bonusDuration > 0)
            {
                LevelTemporaryUtility.activatedTemporaryAtkSpeOnMove = true;
                if (coroutineTempAtkSpeOnMove != null) StopCoroutine(coroutineTempAtkSpeOnMove);
                coroutineTempAtkSpeOnMove = StartCoroutine(IETemporaryBonus(
                    LevelUtility.BonusInfo.tempAtkSpeBonusOnMove.bonusDuration,
                    () =>
                    {
                        LevelTemporaryUtility.activatedTemporaryAtkSpeOnMove = false;
                    }));
            }
        }

        private IEnumerator IETemporaryBonus(float duration, Action completeCallback)
        {
            yield return new WaitForSeconds(duration);
            completeCallback?.Invoke();
        }
    }
}