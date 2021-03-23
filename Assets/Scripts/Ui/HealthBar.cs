using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.utils;

public class HealthBar : MonoBehaviour
{
    public Image HealthImage;
    private RectTransform _canvasRect;
    private Transform UnitToFollow;

    private IUnit _unit;

    //this is the ui element
    RectTransform UI_Element;
    Vector2 ViewportPosition;
    Vector2 WorldObject_ScreenPosition;
    private int _fullHealthEquivalent = 114;    // HARDCODED

    public void Init(IUnit unit, Transform hpTarget, RectTransform canvas)
    {
        _unit = unit;
        UnitToFollow = hpTarget;
        _canvasRect = canvas;
        UI_Element = this.GetComponent<RectTransform>();
        CalculateHealthBar();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (UnitToFollow != null)
            FollowUnit();
    }

    private void FollowUnit()
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        ViewportPosition = Camera.main.WorldToViewportPoint(UnitToFollow.transform.position);
        WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        UI_Element.anchoredPosition = WorldObject_ScreenPosition;
    }

    public void CalculateHealthBar()
    {
        var percent = __percent.What(_unit.Stats.CurrentHealth, _unit.Stats.Health);  // GetValuePercent
        var currentHealthEquivalent = __percent.Find(_fullHealthEquivalent, percent);      // GetPercent
        HealthImage.rectTransform.sizeDelta = new Vector2(currentHealthEquivalent ,HealthImage.rectTransform.sizeDelta.y);
    }
}