using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private static HealthBarController _this;
    public static HealthBarController _ { get { return _this; } }
    void Awake()
    {
        _this = this;
    }

    public RectTransform CanvasRect;
}
