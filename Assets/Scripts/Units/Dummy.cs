using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IUnit
{

    #region ARCHER

    private Unit.OnHit _onHit;

    #endregion

    #region TESTING PROPERTIES

    #endregion

    #region PUBLIC FUNCTIONS - IUnit,

    [SerializeField]
    private int _index;
    public int Index { get { return _index; } set { _index = value; } }

    private Intell _intell;
    public Intell Intell { get { return _intell; } set { _intell = value; } }

    private Stats _stats;
    public Stats Stats { get { return _stats; } set { _stats = value; } }

    public void Init(Unit.OnHit onHit)
    {
        _onHit = onHit;

        _intell.UnitPrimaryState = UnitPrimaryState.Idle;
        
    }

    public void Attack()
    {

    }

    public void ChangeState(int state)
    {
        ChangeState((UnitPrimaryState)state);
    }

    public void ChangeState(UnitPrimaryState uState = UnitPrimaryState.Idle)
    {
        switch (uState)
        {
            case UnitPrimaryState.Idle:
                break;
            case UnitPrimaryState.Walk:
                break;
            case UnitPrimaryState.Busy:
                break;
            case UnitPrimaryState.Stunned:
                break;
            default:
                break;
        }
    }

    public void YouDeadBro()
    {
    }

    #endregion

    #region FUNCTIONS SPECIFIC TO THIS UNIT

    #endregion
}
