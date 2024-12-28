using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.LevelService;
using UnityEngine;

public class Level_Destiny : MonoBehaviour, ILevel
{
    void ILevel.StartLevel()
    {
        Game._.Player.Unit.Init(0, IAm.Player);
    }
}
