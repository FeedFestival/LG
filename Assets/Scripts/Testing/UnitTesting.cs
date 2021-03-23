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
        DummyUnit.Intell.IsImobilized = true;
        DummyUnit.Intell.SetActiveSenses(false);

        TestUnit.Init(0, IAm.Ally);
        TestUnit.Intell.SetActiveSenses(false);
        // UiController._.UnitTestingPanel.SetActive(true);
    }

    public void AttackDummy()
    {
        Fight._.TestFight(TestUnit, DummyUnit);

        //DummyUnit.Intell.SetActiveSenses(false);
        TestUnit.Intell.SetActiveSenses(true);
    }
}
