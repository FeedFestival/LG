using UnityEngine;
using System.Collections;

public class Place
{
    public enum PlaceState
    {
        Idle,
        Hover,
        Full
    }

    private int _x;
    private int _y;
    public MouseArea mouseArea;
    public PlaceState State;

    public Material Material;

    Board.DrawUnitCallback _drawUnitCallback;

    public Place(int x, int y, MouseArea mA, Board.DrawUnitCallback drawUnitCallback)
    {
        _x = x; _y = y;

        mouseArea = mA;
        mouseArea.Init(_x, _y, OnMouseOverCallback, OnMouseExitCallback, OnMouseDownCallback);

        mouseArea.gameObject.name = "plc[" + x + ", " + y + "]";

        _drawUnitCallback = drawUnitCallback;

        MeshRenderer meshRenderer = mouseArea.gameObject.GetComponent<MeshRenderer>();
        Material = meshRenderer.material;
        ChangeState(PlaceState.Idle);
    }

    private void OnMouseOverCallback()
    {
        //ChangeState(PlaceState.Hover);
        _drawUnitCallback(_x, _y);
    }

    private void OnMouseDownCallback()
    {
        //ChangeState(PlaceState.Hover);
    }

    private void OnMouseExitCallback()
    {
        //_drawUnitCallback(_x, _y);
    }

    public bool ChangeState(PlaceState state)
    {
        if (State == PlaceState.Full)
            return false;

        State = state;

        if (State == PlaceState.Idle)
            Material.SetTextureScale("_MainTex", new Vector2(0.5f, 1f));
        else
            Material.SetTextureScale("_MainTex", new Vector2(-0.5f, 1f));

        return true;
    }
}
