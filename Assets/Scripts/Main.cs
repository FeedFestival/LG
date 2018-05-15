using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Board Board;

    // Use this for initialization
    void Start()
    {
        // Show credits

        // Show loader

        // after: StartMoving camera towards the start of the game and raise the blocks.
        Game.Instance().CameraController.Init();
        Board.MapController.Move(MapController.PlatformPosition.Up, timeMultiplier: 25f);

        // If user is first time: show Username input

        // Init small tutorial

        Game.Instance().UiController.MainMenuPanel.SetActive(true);
    }


}
