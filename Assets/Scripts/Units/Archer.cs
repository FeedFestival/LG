using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour, IUnit
{
    #region ARCHER

    public Animation Avatar;
    public Animation Cape;
    public Animation Bow;
    public Animation Arrow;

    private Unit.OnHit _onHit;

    public List<Projectile> Arrows;

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
        Arrow.gameObject.SetActive(false);

        if (Stats.AttackSpeed == 0)
            Stats.AttackSpeed = FightUtils.GetDefaultAttackSpeed(Avatar, "Reload_Body", "Attack_Body");

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

    public void ChangeState(UnitPrimaryState uState = UnitPrimaryState.Idle)
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
    private IEnumerator _fireProjectile;
    private IEnumerator _afterAttack;

    private bool _isAttacking;
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

        _isAttacking = true;

        var animTime = AnimationUtils.GetAnimationLength(Avatar, "Reload_Body", basedOnSpeed: true);

        InternalReload(firstAttack: firstAttack);

        Timer._.InternalWait(AfterReload, animTime);
    }

    private void AttackSpeedChanged()
    {
        _previousAttackAttackSpeed = Stats.AttackSpeed;

        var animationSpeed = FightUtils.CalculateAtackSpeed(
            ref Avatar
            , "Reload_Body", "Attack_Body"
            , Stats.AttackSpeed
            //, debug: true
            );

        float animationWeight = 0f;

        AnimationUtils.SetAnimationOptions(ref Bow, "Reload_Bow", animationSpeed, animationWeight);
        AnimationUtils.SetAnimationOptions(ref Bow, "Attack_Bow", animationSpeed, animationWeight);
        AnimationUtils.SetAnimationOptions(ref Cape, "Reload_Body", animationSpeed, animationWeight);
        AnimationUtils.SetAnimationOptions(ref Cape, "Attack_Body", animationSpeed, animationWeight);
        AnimationUtils.SetAnimationOptions(ref Arrow, "Reload_Arrow", animationSpeed, animationWeight);
    }

    private void AfterReload()
    {
        _afterReload = null;

        if (FightUtils.StopAttacking(this))
        {
            StopAttack();
            return;
        }

        Arrow.gameObject.SetActive(false);
        AnimationUtils.PlayAnimation(
        "Attack", Avatar
        , playImediatly: true
        , accessory: Cape, accessoryName: "Body"
        , accessory2: Bow, accessory2Name: "Bow"
        );

        var animTime = AnimationUtils.GetAnimationLength(Avatar, "Attack_Body", basedOnSpeed: true);
        var arrowExpirationTime = animTime / 6;

        Timer._.InternalWait(FireProjectile, arrowExpirationTime);
        // _fireProjectile = Game.WaitForSeconds(arrowExpirationTime, FireProjectile);
        // StartCoroutine(_fireProjectile);

        Timer._.InternalWait(AfterAttack, animTime);
        // _afterAttack = Game.WaitForSeconds(animTime, AfterAttack);
        // StartCoroutine(_afterAttack);
    }

    private void AfterAttack()
    {
        _afterAttack = null;

        if (FightUtils.StopAttacking(this))
        {
            StopAttack();
            return;
        }

        InternalAttack();
    }

    private void StopAttack()
    {
        if (_afterReload != null)
        {
            StopCoroutine(_afterReload);
            _afterReload = null;
        }

        if (_fireProjectile != null)
        {
            StopCoroutine(_fireProjectile);
            _fireProjectile = null;
        }

        if (_afterAttack != null)
        {
            StopCoroutine(_afterAttack);
            _afterAttack = null;
        }

        _isAttacking = false;
    }

    private void FireProjectile()
    {
        Arrow.gameObject.SetActive(false);

        if (Arrows == null)
            Arrows = new List<Projectile>();

        int? projectileIndex = null;
        Projectile arrowP = FightUtils.GetAvailableProjectile(Arrows, CreateArrow, _onHit, FightUtils.OppositeTeam(Intell.IAm), ref projectileIndex);
        if (arrowP == null && projectileIndex != null)
        {
            arrowP = Arrows[projectileIndex.Value];
        }
        else
        {
            Arrows.Add(arrowP);
            projectileIndex = Arrows.Count;
        }
        var pos = Intell._attackController.GetEnemyTargetPosition();

        // fire projectile
        arrowP.Fire(pos, transform.eulerAngles);
    }

    private Projectile CreateArrow(Projectile projectile)
    {
        var go = Instantiate(Resources.Load("Prefabs/ArrowObj", typeof(GameObject)),
                        Vector3.zero, Quaternion.identity
                        ) as GameObject;
        var p = go.GetComponent<Projectile>();
        p.Init(new Vector3(0, 1.82f, 1.4f), new Vector3(110, 0, 0), Stats.AttackRange, transform, Projectile.ProjectileTrajectory.Arc);
        return p;
    }

    private void InternalReload(bool firstAttack = false)
    {
        Arrow.gameObject.SetActive(true);
        AnimationUtils.PlayAnimation(
            "Reload", Avatar
            , playImediatly: !firstAttack
            , accessory: Cape, accessoryName: "Body"
            , accessory2: Bow, accessory2Name: "Bow"
            , accessory3: Arrow, accessory3Name: "Arrow"
            );
    }

    private void InternalIdle()
    {
        StopAttack();

        Arrow.gameObject.SetActive(false);
        AnimationUtils.PlayAnimation(
            "Idle", Avatar
            , accessory: Cape, accessoryName: "Body"
            , accessory2: Bow, accessory2Name: "Bow"
            );
    }

    private void InternalRun()
    {
        StopAttack();

        Arrow.gameObject.SetActive(false);
        AnimationUtils.PlayAnimation(
            "Run", Avatar
            , accessory: Cape, accessoryName: "Body"
            , accessory2: Bow, accessory2Name: "Bow"
            );
    }

    private void InternalDeath()
    {
        _intell.UnitPrimaryState = UnitPrimaryState.Idle;
        _intell.UnitActionState = UnitActionState.Searching;

        StopAttack();

        Avatar.Stop();
        Cape.Stop();
        if (Arrow.gameObject.activeSelf)
            Arrow.Stop();
        Bow.Stop();
    }
    #endregion
}
