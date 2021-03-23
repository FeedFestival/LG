using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stats : MonoBehaviour
{
    public int Health;

    private int currentHealth;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;

            if (healthBar)
                if (currentHealth < 0)
                {
                    healthBar.gameObject.SetActive(false);
                }
                else
                    healthBar.CalculateHealthBar();
        }
    }

    public int AttackDamage;

    [SerializeField]
    private int _attackSpeed;
    public int AttackSpeed
    {
        get { return _attackSpeed; }
        set
        {
            _attackSpeed = value;
        }
    }

    public float AttackRange;

    [SerializeField]
    private float movementSpeed;
    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            movementSpeed = value;
            if (navAgent != null)
                navAgent.speed = MovementSpeed;
        }
    }

    public bool IsDead { get; set; }

    //

    [HideInInspector]
    public HealthBar healthBar;

    private NavMeshAgent navAgent;

    public void Init(Unit unit)
    {
        CurrentHealth = Health;

        if (unit != null)
        {
            healthBar = Fight.CreateHPBar(unit._unit, unit.HealthBarTarget, unit.Intell.IAm);
            navAgent = unit.gameObject.GetComponent<NavMeshAgent>();
            navAgent.speed = MovementSpeed;
        }
    }
}
