using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTesting : MonoBehaviour
{
    public Unit[] PlayerUnits;
    public Unit[] EnemyUnits;

    // Use this for initialization
    void Start()
    {
        Fight.Instance().Init(PlayerUnits, EnemyUnits);

        Fight.Instance().InitUnits();

        Fight.Instance().StartWaveFight();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
