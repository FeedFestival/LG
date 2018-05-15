using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseArea : MonoBehaviour
{
    public delegate void OnMouseOverCallback();

    public delegate void OnMouseExitCallback();

    public delegate void OnMouseButtonCallback();

    private OnMouseOverCallback _mouseOverCallback;
    private OnMouseExitCallback _onMouseExitCallback;
    private OnMouseButtonCallback _mouseButtonCallback;

    public void Init(int x, int y, OnMouseOverCallback mouseOverCallback, OnMouseExitCallback onMouseExitCallback, OnMouseButtonCallback mouseButtonCallback)
    {
        _mouseOverCallback = mouseOverCallback;
        _onMouseExitCallback = onMouseExitCallback;
        _mouseButtonCallback = mouseButtonCallback;
    }

    void OnMouseEnter()
    {
        _mouseOverCallback();
    }

    void OnMouseUpAsButton()
    {
        _mouseButtonCallback();
    }

    void OnMouseExit()
    {
        _onMouseExitCallback();
    }
}
