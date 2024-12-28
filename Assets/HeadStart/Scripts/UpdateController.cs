using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateController : MonoBehaviour
{
    static UpdateController _this;
    public static UpdateController _ { get { return _this; } }

    void Awake()
    {
        _this = this;
    }

    // Update is called once per frame
    void Update()
    {
	    
    }
}
