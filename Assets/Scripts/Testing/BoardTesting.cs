using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTesting : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Board.Instance().MapController.Move(MapController.PlatformPosition.Up, snap: true, reset: true);

        Board.Instance().Init("HUMANS");
    }
}
