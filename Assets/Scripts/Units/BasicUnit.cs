using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.utils;
using Assets.Scripts.Utils;
using UnityEngine;

public class BasicUnit : MonoBehaviour, IUnit
{

    #region ARCHER

    public Animation Avatar;

    private Unit.OnHit _onHit;

    public List<Projectile> Projectiles;

    public List<GameObject> ShrapnellPos;

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

        InternalReload(firstAttack);
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

        AnimationUtils.SetAnimationOptions(ref Avatar, "Reload_Body", animationSpeed, animationWeight);
    }

    private void AfterReload()
    {
        _afterReload = null;

        if (FightUtils.StopAttacking(this))
        {
            StopAttack();
            return;
        }

        AnimationUtils.PlayAnimation(
        "Attack", Avatar
        );

        FireProjectile();

        var animTime = Avatar["Attack_Body"].length;
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
        if (Projectiles == null)
            Projectiles = new List<Projectile>();

        for (var i = 0; i < 3; i++)
        {
            int? projectileIndex = null;
            Projectile sharpnell = FightUtils.GetAvailableProjectile(Projectiles, CreateShrapnell, _onHit, FightUtils.OppositeTeam(Intell.IAm), ref projectileIndex);
            if (sharpnell == null && projectileIndex != null)
            {
                sharpnell = Projectiles[projectileIndex.Value];
            }
            else
            {
                Projectiles.Add(sharpnell);
                projectileIndex = Projectiles.Count;
            }

            // Debug.Log(
            //     __debug.DebugList<GameObject>(ShrapnellPos, "ShrapnellPos", (go) =>
            //     {
            //         return go.name;
            //     })
            // );
            if (__list.isNilOrEmpty(ShrapnellPos) == false)
            {
                sharpnell.Fire(
                    transform.TransformPoint(ShrapnellPos[i].transform.position),
                    transform.eulerAngles);
            }
        }
    }

    private Projectile CreateShrapnell(Projectile projectile)
    {
        var go = Instantiate(Resources.Load("Prefabs/Sharpnell", typeof(GameObject)),
                        Vector3.zero, Quaternion.identity
                        ) as GameObject;
        var p = go.GetComponent<Projectile>();
        p.Init(new Vector3(0, 1.82f, 1.4f), new Vector3(110, 0, 0), Stats.AttackRange, transform);
        return p;
    }

    private void InternalReload(bool firstAttack = false)
    {
        var animTime = Avatar["Reload_Body"].length;

        AnimationUtils.PlayAnimation(
            "Reload", Avatar
            , playImediatly: !firstAttack
            );

        Timer._.InternalWait(AfterReload, animTime);
        // _afterReload = Game.WaitForSeconds(animTime, AfterReload);
        // StartCoroutine(_afterReload);
    }

    private void InternalIdle()
    {
        StopAttack();

        //Avatar.CrossFade("Fire_Prep");

        AnimationUtils.PlayAnimation(
            "Idle", Avatar
            , accessory: Avatar, accessoryName: "Body"
            );
    }

    private void InternalRun()
    {
        StopAttack();

        //Avatar.CrossFade("Run");

        AnimationUtils.PlayAnimation(
            "Run", Avatar
            , accessory: Avatar, accessoryName: "Body"
            );
    }

    private void InternalDeath()
    {
        _intell.UnitPrimaryState = UnitPrimaryState.Idle;
        _intell.UnitActionState = UnitActionState.Searching;

        StopAttack();

        Avatar.Stop();
    }
    #endregion

    //private const string Idle = "Fire_Prep";
    //private const string Walk = "Run";
    //private const string Reload = "Reload";
    //private const string FirePrep = "Fire_Prep";
    //private const string AttackForward = "Fire";
    //private const string MiniStun = "Mini_Stun";

    //private bool directionUp;
    //private Vector3 _directionUp = new Vector3(0,0,0);
    //private Vector3 _directionDown = new Vector3(0, 180, 0);

    //UnitPrimaryState _statePriorToStun;

    //private IEnumerator _firePrepTimer;
    //private IEnumerator _reloadTimer;
    //private IEnumerator _fireTimer;
    //private IEnumerator _stunTimer;

    //private void AfterPrep()
    //{
    //    _firePrepTimer = null;

    //    if (_intell.UnitPrimaryState == UnitPrimaryState.Stunned)
    //        return;
    //    if (_intell.UnitPrimaryState != UnitPrimaryState.Busy || _intell.UnitActionState != UnitActionState.Attacking)
    //        return;

    //    var fireTime = Avatar[AttackForward].length;
    //    Avatar.Play(AttackForward);

    //    StartCoroutine(Game.WaitForSeconds(fireTime / 2, () =>
    //    {
    //        _onHit();
    //    })
    //    );

    //    _fireTimer = Game.WaitForSeconds(fireTime, AfterFire);
    //    StartCoroutine(_fireTimer);
    //}

    //private void AfterFire()
    //{
    //    _fireTimer = null;
    //    _isReloaded = false;

    //    if (_intell.UnitPrimaryState == UnitPrimaryState.Stunned)
    //        return;
    //    if (_intell.UnitPrimaryState == UnitPrimaryState.Busy && _intell.UnitActionState == UnitActionState.Attacking)
    //        Attack();
    //}

    //private void GoStun()
    //{
    //    if (_firePrepTimer != null)
    //        StopCoroutine(_firePrepTimer);
    //    if (_reloadTimer != null)
    //        StopCoroutine(_reloadTimer);
    //    if (_fireTimer != null)
    //        StopCoroutine(_fireTimer);
    //    if (_stunTimer != null)
    //        StopCoroutine(_stunTimer);

    //    var stunTime = Avatar[MiniStun].length - (Avatar[MiniStun].length / 4);
    //    Avatar.CrossFade(MiniStun);
    //    _stunTimer = Game.WaitForSeconds(stunTime, AfterStun);
    //    StartCoroutine(_stunTimer);
    //}

    //private void AfterStun()
    //{
    //    _stunTimer = null;

    //    _intell.UnitPrimaryState = _statePriorToStun;
    //    if (_intell.UnitPrimaryState == UnitPrimaryState.Busy && _intell.UnitActionState == UnitActionState.Attacking)
    //        Attack();
    //}

}
