using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMan : MonoBehaviour, IUnit
{
    #region AVATAR PROPERTIES
    public Animation Avatar;
    public Animation Shield;
    public Animation Weapon;

    public HitArea AxeHitArea;

    public int? AttackPhase;
    private int previousStartAttackPhase;
    private bool _isAttacking;
    public enum ManWeapon
    {
        Axe,
        Sword,
        MiniLance
    }
    [SerializeField]
    private ManWeapon _shieldManWeapon;
    public ManWeapon ShieldManWeapon
    {
        get { return _shieldManWeapon; }
        set { _shieldManWeapon = value; }
    }
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
        _intell.UnitPrimaryState = UnitPrimaryState.Idle;

        AxeHitArea.Init(onHit, Intell.IAm == IAm.Ally ? IAm.Enemy.ToString() : IAm.Ally.ToString());
        AxeHitArea.gameObject.SetActive(false);

        if (Stats.AttackSpeed == 0)
            Stats.AttackSpeed = FightUtils.GetDefaultAttackSpeed(Avatar, "Reload_Body_2", "Attack_Body_2");

        AttackSpeedChanged();
    }

    public void Attack()
    {
        if (_isAttacking)
            return;

        InternalAttack();
    }

    public void ChangeState(int state)
    {
        ChangeState((UnitPrimaryState)state);
    }

    public void ChangeState(UnitPrimaryState uState)
    {
        switch (uState)
        {
            case UnitPrimaryState.Idle:
                InternalIdle();
                break;
            case UnitPrimaryState.Walk:
                InternalRun();
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
        InternalDeath();
    }

    #endregion

    #region FUNCTIONS SPECIFIC TO THIS UNIT

    private IEnumerator _afterReload;
    private IEnumerator _afterAttack;

    private int _previousAttackAttackSpeed;

    private void InternalAttack()
    {
        var firstAttack = _intell.UnitActionState != UnitActionState.Attacking;

        _intell.UnitPrimaryState = UnitPrimaryState.Busy;
        _intell.UnitActionState = UnitActionState.Attacking;

        if (Avatar == null)
            return;

        if (_previousAttackAttackSpeed != Stats.AttackSpeed)
            AttackSpeedChanged();

        // Figure out atackType
        if (AttackPhase == null)
        {
            _isAttacking = true;
            AttackPhase = System.Convert.ToInt32(Random.Range(1, 3));
            if (AttackPhase.Value == previousStartAttackPhase)
            {
                AttackPhase = 3;
            }
            previousStartAttackPhase = AttackPhase.Value;
        }

        var animTime = AnimationUtils.GetAnimationLength(Avatar, "Reload_Body_" + AttackPhase, basedOnSpeed: true);

        InternalReload(firstAttack: firstAttack);

        Timer._.InternalWait(AfterReload, animTime);
        // _afterReload = Game.WaitForSeconds(animTime, AfterReload);
        // StartCoroutine(_afterReload);
    }

    private void AfterReload()
    {
        _afterReload = null;

        if (FightUtils.StopAttacking(this))
        {
            StopAttack();
            return;
        }

        AnimationUtils.PlayAnimation("Attack", Avatar
            , attackPhase: AttackPhase, singleVariation: false
            , playImediatly: true
            , accessory: Weapon, accessoryName: "Axe"
            , accessory2: Shield, accessory2Name: "Shield");

        AxeHitArea.gameObject.SetActive(true);
        AxeHitArea.Reset();

        var animTime = AnimationUtils.GetAnimationLength(Avatar, "Attack_Body_" + AttackPhase.Value, basedOnSpeed: true);

        Timer._.InternalWait(AfterAttack, animTime);
        // _afterAttack = Game.WaitForSeconds(animTime, AfterAttack);
        // StartCoroutine(_afterAttack);
    }

    private void AfterAttack()
    {
        _afterAttack = null;

        AxeHitArea.gameObject.SetActive(false);

        if (FightUtils.StopAttacking(this))
        {
            StopAttack();
            return;
        }

        if (AttackPhase.Value == 3)
            AttackPhase = 1;
        else
            AttackPhase++;

        InternalAttack();
    }

    private void StopAttack()
    {
        if (_afterReload != null)
        {
            StopCoroutine(_afterReload);
            _afterReload = null;
        }

        if (_afterAttack != null)
        {
            StopCoroutine(_afterAttack);
            _afterAttack = null;
        }

        _isAttacking = false;
        AttackPhase = null;
    }

    private void InternalReload(bool firstAttack = false)
    {
        AnimationUtils.PlayAnimation("Reload", Avatar
            , attackPhase: AttackPhase, singleVariation: false
            , playImediatly: !firstAttack
            , accessory: Weapon, accessoryName: "Axe"
            , accessory2: Shield, accessory2Name: "Shield");
    }

    private void AttackSpeedChanged()
    {
        _previousAttackAttackSpeed = Stats.AttackSpeed;

        var animationWeight = 0f;

        for (var i = 1; i <= 3; i++)
        {
            var animationSpeed = FightUtils.CalculateAtackSpeed(
            ref Avatar
            , "Reload_Body_" + i, "Attack_Body_" + i
            , Stats.AttackSpeed
            );

            AnimationUtils.SetAnimationOptions(ref Shield, "Reload_Shield_" + i, animationSpeed, animationWeight);
            AnimationUtils.SetAnimationOptions(ref Shield, "Attack_Shield_" + i, animationSpeed, animationWeight);
            AnimationUtils.SetAnimationOptions(ref Weapon, "Reload_Axe_" + i, animationSpeed, animationWeight);
            AnimationUtils.SetAnimationOptions(ref Weapon, "Attack_Axe_" + i, animationSpeed, animationWeight);
        }
    }

    private void InternalIdle()
    {
        StopAttack();

        AnimationUtils.PlayAnimation("Idle", Avatar
            , accessory: Weapon, accessoryName: "Axe"
            , accessory2: Shield, accessory2Name: "Shield");
    }

    private void InternalRun()
    {
        StopAttack();

        AnimationUtils.PlayAnimation("Run", Avatar
            , accessory: Weapon, accessoryName: "Axe"
            , accessory2: Shield, accessory2Name: "Shield");
    }

    private void InternalDeath()
    {
        _intell.UnitPrimaryState = UnitPrimaryState.Idle;
        _intell.UnitActionState = UnitActionState.Searching;

        StopAttack();

        Avatar.Stop();
        Shield.Stop();
        Weapon.Stop();
    }
    #endregion
}
