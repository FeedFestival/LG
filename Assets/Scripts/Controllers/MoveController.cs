using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    public GameObject UnitTarget;

    [HideInInspector]
    public Intell Intell;
    public TriggerListener MovementSensor;

    private NavMeshAgent _navAgent;
    private IUnit _unit;

    private IEnumerator _followTarget;
    [SerializeField]
    private Vector3 _lastTarget;
    public float WalkTurnSpeed;
    private int? _steerTid;

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
        {
            _navAgent.enabled = false;
            _navAgent.updateRotation = false;
        }

        if (UnitTarget == null)
        {
            UnitTarget = WorldUtils.CreateUnitTarget(unitName);
        }

        if (MovementSensor == null)
        {
            Debug.LogError(unitName + " - has no Movement Sensor");
        }
        else
        {
            MovementSensor.Init("Navigation", true, (_) =>
            {
                StopMoving(true);
            });
        }
    }

    void LateUpdate()
    {
        if (_navAgent == null || _unit == null) { return; }

        if (_navAgent.path.corners != null && _navAgent.path.corners.Length > 1)
        {
            if (_lastTarget == _navAgent.path.corners[1])
            {
                return;
            }
            _lastTarget = _navAgent.path.corners[1];

            Vector3 dir = (_lastTarget - transform.position);
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            SteerWalkingDirection(rot);
        }
    }

    public void SteerWalkingDirection(Quaternion rot)
    {
        if (_steerTid.HasValue)
        {
            LeanTween.cancel(_steerTid.Value);
        }
        _steerTid = LeanTween.rotate(gameObject, rot.eulerAngles, WalkTurnSpeed).id;
        LeanTween.descr(_steerTid.Value).setEase(LeanTweenType.easeOutCubic);
        LeanTween.descr(_steerTid.Value).setOnComplete(() =>
        {
            _steerTid = null;
        });
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

            // StopCoroutine(_lerpRotation);
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
            // TODO: get the last Endpoint
            // - And show the move indicator there;

            _navAgent.SetDestination(pos);

            // var debugPath = __debug.DebugList<Vector3>(_navAgent.path.corners.ToList(), "debugPath", (Vector3 p) =>
            // {
            //     return p.ToString();
            // });
            // Debug.Log("debugPath: " + debugPath);
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
