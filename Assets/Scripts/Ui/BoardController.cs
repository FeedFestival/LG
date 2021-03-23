using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private static BoardController _this;
    public static BoardController _ { get { return _this; } }
    void Awake()
    {
        _this = this;
    }

    public RectTransform BoardListPanel;
}
