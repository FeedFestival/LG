using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private IUnit _unit;
    private MoveController _moveController;

    [HideInInspector]
    public Intell Intell;

    private IAm _targetEnemyTeam;
    private int? _targetEnemyIndex;
    
    public void Init(
        IUnit unit,
        MoveController moveController
        )
    {
        _unit = unit;
        _moveController = moveController;
    }

    public void EnemyInRange(GameObject unitGo = null)
    {
        if (unitGo == null)
            return;

        var unit = unitGo.GetComponent<Unit>();
        if (unit.Stats.IsDead)
            return;
        _targetEnemyIndex = unit._unit.Index;
        _targetEnemyTeam = unit.Intell.IAm;
        
        //Debug.Log(gameObject.name + ": enemy (" + TargetEnemy.gameObject.name + ") is in range.");

        _moveController.StopMoving(false);
        _unit.Attack();

        FightUtils.FaceEnemy(unitGo.transform.position, transform, onlyY: false);
        
        Intell.AtackSensor.gameObject.SetActive(false);
    }

    public void OnHit(
        int damage = 0,
        int targetIndex = 0,
        IAm targetIndexTeam = IAm.Ally
        )
    {
        if (_targetEnemyIndex != null)
        {
            var targetEnemy = Fight._.GetUnit(_targetEnemyTeam, _targetEnemyIndex.Value);

            var curHealth = targetEnemy.Stats.CurrentHealth - _unit.Stats.AttackDamage;
            if (curHealth < 0)
            {
                KillTarget(targetEnemy);
            }
            else
                targetEnemy.Stats.CurrentHealth = curHealth;
        }
        else if (Intell.IsEnemyInViewRange() == false)
            Fight.ExecuteOrder66(GetComponent<Unit>(), ally: Intell.IAm == IAm.Ally);
    }

    public void TrySetTarget(GameObject unitGo)
    {
        if (_targetEnemyIndex == null)
        {
            var unit = unitGo.GetComponent<Unit>();
            if (unit.Stats.IsDead)
                return;
            _targetEnemyIndex = unit._unit.Index;
            _targetEnemyTeam = unit.Intell.IAm;
        }

        if (_targetEnemyIndex == null)
            return;

        //Debug.Log(gameObject.name + ": I will deal with the enemy (" + TargetEnemy.gameObject.name + ")");

        var targetEnemy = Fight._.GetUnit(_targetEnemyTeam, _targetEnemyIndex.Value);

        var distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
        if (_unit.Stats.AttackRange >= distance)
        {
            EnemyInRange(unitGo);
        }
        else
        {
            Intell.AtackSensor.gameObject.SetActive(true);
            _moveController.FollowObject(targetEnemy.transform);
        }
    }

    public Vector3 GetEnemyTargetPosition()
    {
        if (_targetEnemyIndex == null)
        {
            return transform.forward * _unit.Stats.AttackRange + transform.position;
        }
        return Fight._.GetUnit(_targetEnemyTeam, _targetEnemyIndex.Value).transform.position;
    }

    private void KillTarget(Unit targetEnemy)
    {
        // 1. put the moveTarget in the world space.
        _moveController.UnitTarget.transform.SetParent(null);

        // 2. set enemy stats as Dead
        targetEnemy.Stats.IsDead = true;
        _targetEnemyIndex = null;
        targetEnemy.Stats.CurrentHealth = 0;

        // 3. tell enemy it is dead :D.
        targetEnemy._unit.YouDeadBro();

        // 4. look fast for another enemy.
        if (Intell.IsEnemyInViewRange() == false)
            Fight.ExecuteOrder66(GetComponent<Unit>(), ally: Intell.IAm == IAm.Ally);
    }
}
