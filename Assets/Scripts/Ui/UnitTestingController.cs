using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTestingController : MonoBehaviour
{
    private static UnitTestingController _this;
    public static UnitTestingController _ { get { return _this; } }
    void Awake()
    {
        _this = this;
    }
}
