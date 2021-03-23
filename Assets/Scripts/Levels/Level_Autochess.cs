using UnityEngine;
using Assets.Scripts.LevelService;

public class Level_Autochess : MonoBehaviour, ILevel
{
    public Unit[] PlayerUnits;
    public Unit[] EnemyUnits;
    public AutochessMode AutochessMode;

    public void StartLevel()
    {
        if (AutochessMode == AutochessMode.Board)
        {
            Board._.MapController.Move(MapController.PlatformPosition.Up, snap: true, reset: true);
            Board._.Init("HUMANS");
            return;
        }

        Fight._.Init(PlayerUnits, EnemyUnits);
        Fight._.InitUnits();
        Fight._.StartWaveFight();
    }

}

public enum AutochessMode
{
    Fight, Board
}