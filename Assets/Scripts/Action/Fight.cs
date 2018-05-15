﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fight : MonoBehaviour
{
    /*
     * Singleton FIGHT!
     */
    private static Fight _fight;
    public static Fight Instance()
    {
        return _fight;
    }
    void Awake()
    {
        _fight = this;
        PlayerUnits = null;
        EnemyUnits = null;
    }

    public Transform EnemyObjectivePoint;
    public Transform AllyObjectivePoint;

    [HideInInspector]
    public List<Unit> PlayerUnits;
    [HideInInspector]
    public List<Unit> EnemyUnits;

    internal void TestFight(Unit testUnit, Unit dummyUnit)
    {
        if (PlayerUnits == null)
            PlayerUnits.Add(testUnit);
        else
            PlayerUnits[0] = testUnit;

        if (EnemyUnits == null)
            EnemyUnits.Add(dummyUnit);
        else
            EnemyUnits[0] = dummyUnit;

        ExecuteOrder66(testUnit, dummyUnit.transform);
    }

    public void Init(Unit[] playerUnits, Unit[] enemyUnits)
    {
        if (playerUnits != null)
            PlayerUnits = playerUnits.ToList();
        EnemyUnits = enemyUnits.ToList();
    }

    public void AddPlayerUnit(Unit unit)
    {
        if (PlayerUnits == null)
            PlayerUnits = new List<Unit>();
            PlayerUnits.Add(unit);
    }

    public void InitUnits()
    {
        int i = 0;
        foreach (Unit unit in PlayerUnits)
        {
            InitUnit(i, unit, IAm.Ally);
            i++;
        }

        i = 0;
        foreach (Unit unit in EnemyUnits)
        {
            InitUnit(i, unit, IAm.Enemy);
            i++;
        }

        // TODO: this should be moved.
        Game.Instance().UiController.Init();
    }

    public void InitUnit(int index, Unit unit, IAm iAm)
    {
        if (unit)
            unit.Init(index, iAm);
        else
            Debug.Log("No " + iAm.ToString() + " unit !");
    }

    public void StartWaveFight()
    {
        if (PlayerUnits != null && PlayerUnits.Count > 0)
            foreach (Unit unit in PlayerUnits)
            {
                if (unit == null)
                    continue;

                ExecuteOrder66(unit, AllyObjectivePoint);
            }

        if (EnemyUnits != null && EnemyUnits.Count > 0)
            foreach (Unit unit in EnemyUnits)
            {
                if (unit == null)
                    continue;

                ExecuteOrder66(unit, EnemyObjectivePoint);
            }

        //
        Game.Instance().CameraController.StartFight();
    }

    public static void ExecuteOrder66(Unit unit, Transform objective = null, bool ally = true)
    {
        var xPos = unit.transform.position.x;
        Vector3 destination;
        if (objective == null)
            objective = ally ? _fight.AllyObjectivePoint : _fight.EnemyObjectivePoint;
        destination = new Vector3(xPos, objective.position.y, objective.position.z);
        unit.MoveController.SetDestination(destination);
    }

    public static HealthBar CreateHPBar(IUnit unit, Transform hpTarget, IAm iAm)
    {
        if (Game.Instance().UiController.HealthBarsPanel.gameObject.activeSelf == false)
            Game.Instance().UiController.HealthBarsPanel.gameObject.SetActive(true);

        GameObject go = Instantiate(
            Resources.Load("Prefabs/UI/HealthBar", typeof(GameObject)),
            Vector3.zero, Quaternion.identity, Game.Instance().UiController.HealthBarsPanel.transform
            ) as GameObject;
        var rect = go.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localEulerAngles = Vector3.zero;

        var hp = go.GetComponent<HealthBar>();
        var hpColor = (iAm == IAm.Ally) ? Game.Instance().PlayerHpBars : Game.Instance().EnemyHpBars;
        hp.HealthImage.color = hpColor;
        hp.Init(unit, hpTarget, Game.Instance().UiController.Canvas);
        return hp;
    }

    public Unit GetUnit(IAm targetEnemyTeam, int targetEnemyIndex)
    {
        if (targetEnemyTeam == IAm.Ally)
            return Instance().PlayerUnits[targetEnemyIndex];
        return Instance().EnemyUnits[targetEnemyIndex];
    }
}
