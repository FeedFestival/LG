using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMouse PlayerMouse;
    public PlayerIntention PlayerIntention;
    //--------------------------------------------
    public Unit Unit;
    public MoveIndicator MoveIndicator;

    public void ShowMoveIndicator(Vector3 pos)
    {
        MoveIndicator.transform.position = pos;
        MoveIndicator.Play();
    }
}

public enum PlayerMouse
{
    None
}

public enum PlayerIntention
{
    None
}
