using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;

public interface IUnit
{
    int Index { get; set; }

    Intell Intell { get; set; }
    Stats Stats { get; set; }

    void Init(Unit.OnHit onHit);
    void Attack();
    void ChangeState(UnitPrimaryState state);
    void YouDeadBro();
}

public enum UnitPrimaryState
{
    Idle,
    Walk,
    Busy,
    Stunned
}

public enum UnitActionState
{
    Searching,
    Attacking
}

public enum UnitActionInMind
{
    None
}

public enum IAm
{
    Enemy,
    Ally
}