using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    public GameObject UnitTarget;

    [HideInInspector]
    public Intell Intell;

    private NavMeshAgent _navAgent;
    private IUnit _unit;

    private IEnumerator _followTarget;

    private bool _lerpRotComplete = true;
    private Vector3 _lastTarget;

    private float _walkTurnSpeed;
    //private float _turnSpeed;
    private float _lerpTime;

    IEnumerator _lerpRotation;

    public void Init(
        string unitName,
        NavMeshAgent navAgent,
        IUnit unit,
        bool startImobilized
        )
    {
        _navAgent = navAgent;
        _unit = unit;

        if (startImobilized)
            _navAgent.enabled = false;

        // Init Values
        _walkTurnSpeed = 11.0f;
        //_turnSpeed = 4.0f;
        _lerpTime = 0.6f;

        _lerpRotation = LerpToRotation();

        if (UnitTarget == null)
        {
            UnitTarget = WorldUtils.CreateUnitTarget(unitName);
        }
    }

    void Update()
    {
        if (_unit == null)
            return;

        if (Intell.UnitPrimaryState == UnitPrimaryState.Walk)
        {
            SteerWalkingDirection();
        }
    }

    public void SteerWalkingDirection()
    {
        if (_lastTarget != _navAgent.steeringTarget)
        {
            _lastTarget = _navAgent.steeringTarget;

            if (_lerpRotComplete == true)
                StartCoroutine(_lerpRotation);
        }
        if (_lerpRotComplete == false && _lastTarget != Vector3.zero)
        {
            transform.rotation = WorldUtils.SmoothLook(
                transform.rotation,
                WorldUtils.GetDirection(transform.position, _lastTarget),
                _walkTurnSpeed);
        }
    }

    private IEnumerator LerpToRotation()
    {
        _navAgent.updateRotation = false;
        _lerpRotComplete = false;

        yield return new WaitForSeconds(_lerpTime);

        _lerpRotComplete = true;
        _navAgent.updateRotation = true;
    }

    public void StopMoving(bool targetReached = true)
    {
        if (_navAgent.enabled == false)
            _navAgent.enabled = true;
        _navAgent.isStopped = true;

        if (targetReached)
        {
            //  If we finish the journey
            //      - And an action is set in mind
            //      - And the Player is not busy with another action.
            //  --> That means we must fire the Action in mind and exit.
            if (Intell.UnitActionInMind != UnitActionInMind.None && Intell.UnitPrimaryState != UnitPrimaryState.Busy)
            {
                _navAgent.enabled = false;
                //_unit.UnitActionHandler.StartAction();
                UnitTarget.SetActive(false);
                return;
            }

            //  If we finish the journey
            //      - And for some reason the unit is Busy - dont go into Attack.
            if (Intell.UnitPrimaryState != UnitPrimaryState.Busy)
            {
                Intell.SetPrimaryState(UnitPrimaryState.Idle, changeState: true);
                //_unit.UnitBasicAnimation.Play(_unit.UnitPrimaryState);
            }
        }
        else
        {
            Intell.SetPrimaryState(UnitPrimaryState.Idle, changeState: true);
            //_unit.UnitBasicAnimation.Play(_unit.UnitPrimaryState);

            StopCoroutine(_lerpRotation);
            UnitTarget.transform.position = _navAgent.gameObject.transform.position;
        }

        UnitTarget.SetActive(false);
    }

    public void ResumeMoving()
    {
        if (_navAgent.enabled == false)
            _navAgent.enabled = true;
        JustMove(UnitTarget.transform.position);
        _navAgent.isStopped = false;

        Intell.SetPrimaryState(UnitPrimaryState.Walk, changeState: true);
    }

    private void JustMove(Vector3 pos)
    {
        try
        {
            _navAgent.SetDestination(pos);
        }
        catch (System.Exception e)
        {
            Debug.LogError(gameObject.name + ": " + e);
        }
    }

    public void GoToTarget()
    {
        if (Intell.IsImobilized)
            return;

        UnitTarget.SetActive(true);
        ResumeMoving();
    }

    public void SetDestination(Vector3 destination)
    {
        //_targetIsMoving = false;

        UnitTarget.transform.position = destination;
        GoToTarget();
    }

    public void FollowObject(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            Debug.LogWarning(gameObject.name + " is Following " + targetTransform.gameObject.name + " which is dead ?");
            return;
        }

        UnitTarget.transform.position = targetTransform.position;
        UnitTarget.transform.SetParent(targetTransform);

        GoToTarget();

        _followTarget = FollowTarget();
        StartCoroutine(_followTarget);
    }

    public IEnumerator FollowTarget()
    {
        while (_followTarget != null)
        {
            yield return new WaitForSeconds(0.1f);

            try
            {
                JustMove(UnitTarget.transform.position);
            }
            catch (System.Exception e)
            {
                Debug.LogError(gameObject.name + " - UnitTarget is gone. :( : " + e);
            }
        }
    }
}
