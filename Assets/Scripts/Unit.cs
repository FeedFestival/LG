﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public delegate void OnHit(
        int damage = 0,
        int targetIndex = 0,
        IAm targetIndexTeam = IAm.Ally
        );

    [HideInInspector]
    public Stats Stats;

    [HideInInspector]
    public MoveController MoveController;
    private AttackController _attackController;

    [HideInInspector]
    public UnitInteligence UnitInteligence;

    public IUnit _unit;

    public string UnitName;

    public Transform HealthBarTarget;

    public int DataId;

    /// <summary>
    ///                                                                         THE ORDER HERE IS VERY IMPORTANT !!!!!
    /// </summary>
    /// <param name="iAm"></param>
    public void Init(int index, IAm iAm = IAm.Ally, bool startImobilized = false)
    {
        _unit = GetComponent<IUnit>();

        _unit.Index = index;

        SetupInteligence(iAm);

        SetupStats();

        SetupMovement(startImobilized);

        SetupAttack();

        _unit.Init(_attackController.OnHit);

        //var navAgent = GetComponent<NavMeshAgent>();
        //if (navAgent == null)
        //{
        //    Debug.LogError("Unit has no navAgent. You can't move without it.");
        //    return;
        //}
        //MoveController.Init(UnitName, navAgent, _unit, startImobilized);
        //MoveController.StopMoving(targetReached: false);
    }

    public void BoardSetup(int unitId) {
        DataId = unitId;

        _unit = GetComponent<IUnit>();

        SetupInteligence(IAm.Ally);

        SetupStats();

        _unit.Init(null);

        DisableColliders();
        DisableNavMeshAgent();
    }

    public void DisableColliders()
    {
        if (UnitInteligence == null)
            UnitInteligence = GetComponent<UnitInteligence>();
        UnitInteligence.AtackSensor.gameObject.SetActive(false);
        UnitInteligence.ViewSensor.gameObject.SetActive(false);

        GetComponent<CapsuleCollider>().enabled = false;
    }

    public void DisableNavMeshAgent()
    {
        var navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
    }

    private void SetupInteligence(IAm iAm) {
        UnitInteligence = GetComponent<UnitInteligence>();
        if (UnitInteligence == null)
        {
            Debug.LogError("Unit has no inteligence.");
            return;
        }
        _unit.UnitInteligence = UnitInteligence;
        UnitInteligence.IAm = iAm;
        UnitInteligence.Init(_unit);
        gameObject.tag = iAm.ToString();
    }

    private void SetupStats()
    {
        Stats = GetComponent<Stats>();
        if (Stats == null)
        {
            Debug.LogError("Unit has no Stats.");
            return;
        }
        _unit.Stats = this.Stats;
        Stats.Init(this);
    }


    private void SetupAttack()
    {
        _attackController = GetComponent<AttackController>();
        if (_attackController == null)
        {
            Debug.LogError("Unit has no Attack controller.");
            return;
        }
        UnitInteligence.SetupAtackSensor(_attackController);
        _attackController.UnitInteligence = UnitInteligence;
        _attackController.Init(_unit, MoveController);
    }

    private void SetupMovement(bool startImobilized)
    {
        MoveController = GetComponent<MoveController>();
        if (MoveController == null)
        {
            Debug.LogError("Unit has no Move controller.");
            return;
        }
        MoveController.UnitInteligence = UnitInteligence;
        UnitInteligence._moveController = MoveController;
        //
        var navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("Unit has no navAgent. You can't move without it.");
            return;
        }
        MoveController.Init(UnitName, navAgent, _unit, startImobilized);
        MoveController.StopMoving(targetReached: false);
    }
}
