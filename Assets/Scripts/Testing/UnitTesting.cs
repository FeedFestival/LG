using Assets.Scripts.LevelService;
using UnityEngine;

public class UnitTesting : MonoBehaviour, ILevel
{
    public Unit TestUnit;
    public Unit DummyUnit;

    // Use this for initialization
    public void StartLevel()
    {
        DummyUnit.Init(0, IAm.Enemy);
        DummyUnit.Intell.IsImobilized = true;
        DummyUnit.Intell.SetActiveSenses(false);

        TestUnit.Init(0, IAm.Ally);
        TestUnit.Intell.SetActiveSenses(false);
        UnitTestingController._.gameObject.SetActive(true);
    }

    public void AttackDummy()
    {
        Fight._.TestFight(TestUnit, DummyUnit);

        //DummyUnit.Intell.SetActiveSenses(false);
        TestUnit.Intell.SetActiveSenses(true);
    }
}
