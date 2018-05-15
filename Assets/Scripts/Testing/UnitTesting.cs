using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTesting : MonoBehaviour
{
    public Unit TestUnit;
    public Unit DummyUnit;

    // Use this for initialization
    void Start()
    {
        DummyUnit.Init(0, IAm.Enemy);
        DummyUnit.UnitInteligence.IsImobilized = true;
        DummyUnit.UnitInteligence.SetActiveSenses(false);

        TestUnit.Init(0, IAm.Ally);
        TestUnit.UnitInteligence.SetActiveSenses(false);
        Game.Instance().UiController.UnitTestingPanel.SetActive(true);
    }

    public void AttackDummy()
    {
        Fight.Instance().TestFight(TestUnit, DummyUnit);

        //DummyUnit.UnitInteligence.SetActiveSenses(false);
        TestUnit.UnitInteligence.SetActiveSenses(true);
    }
}
