using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class Intell : MonoBehaviour
{
    private IUnit _unit;
    [HideInInspector]
    public MoveController _moveController;
    [HideInInspector]
    public AttackController _attackController;

    [SerializeField]
    private UnitPrimaryState _unitPrimaryState;
    public UnitPrimaryState UnitPrimaryState
    {
        get
        {
            return _unitPrimaryState;
        }
        set
        {
            _unitPrimaryState = value;
            if (_unit != null)
                _unit.ChangeState(_unitPrimaryState);
        }
    }

    [SerializeField]
    private UnitActionState _unitActionState;
    public UnitActionState UnitActionState
    {
        get { return _unitActionState; }
        set { _unitActionState = value; }
    }

    [SerializeField]
    private UnitActionInMind _unitActionInMind;
    public UnitActionInMind UnitActionInMind
    {
        get { return _unitActionInMind; }
        set { _unitActionInMind = value; }
    }

    [SerializeField]
    private IAm _iAm;
    public IAm IAm
    {
        get { return _iAm; }
        set { _iAm = value; }
    }

    public TriggerListener ViewSensor;
    public TriggerListener AtackSensor;

    [Header("External States")]
    public bool IsImobilized;

    public void Init(
        IUnit unit
        )
    {
        // vars
        _unit = unit;

        // sensors
        var lookFor = IAm == IAm.Ally ? IAm.Enemy : IAm.Ally;

        if (gameObject.GetComponent<Rigidbody>() == null)
            Debug.LogError("I have no Rigidbody, I can't be seen by sensors!");

        if (gameObject.GetComponent<CapsuleCollider>() == null)
            Debug.LogError("I have no CapsuleCollider, I can't be seen by sensors!");

        if (ViewSensor == null)
            Debug.LogError("I have no view Sensor !");
        else
        {
            ViewSensor.Init(lookFor.ToString(), true, EnemyInViewRange);
        }
    }

    public void SetActiveSenses(bool value)
    {
        ViewSensor.gameObject.SetActive(value);
        AtackSensor.gameObject.SetActive(value);
    }

    public void SetupAtackSensor(AttackController atkController)
    {
        _attackController = atkController;

        if (AtackSensor == null)
            Debug.LogError("I have no attack Sensor !");
        else
        {
            var lookFor = IAm == IAm.Ally ? IAm.Enemy : IAm.Ally;

            AtackSensor.Init(lookFor.ToString(), true, _attackController.EnemyInRange);
            AtackSensor.transform.localScale = new Vector3(_unit.Stats.AttackRange * 2, _unit.Stats.AttackRange * 2, _unit.Stats.AttackRange * 2);
            AtackSensor.gameObject.SetActive(false);
        }
    }

    public void EnemyInViewRange(GameObject unitGo = null)
    {
        if (unitGo == null)
            return;

        // here I need to have a list of enemies that enter the field of view and exit it, easy it will be with Unit.Index

        _attackController.TrySetTarget(unitGo);
    }

    public bool IsEnemyInViewRange()
    {
        var maxdistance = 16f;
        Unit target = null;
        var enemies = IAm == IAm.Ally ? Fight._.EnemyUnits : Fight._.PlayerUnits;
        foreach (Unit enemy in enemies)
        {
            if (enemy == null || enemy.Stats.IsDead)
                continue;

            var distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < maxdistance)
            {
                maxdistance = distance;
                target = enemy;
            }
        }

        if (target == null)
            return false;

        _attackController.TrySetTarget(target.gameObject);

        return true;
    }
}
