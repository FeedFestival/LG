using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    public bool DebugThis;

    public delegate void OnEnter(GameObject gameObject = null);
    private OnEnter _onEnter;

    private string _tagToCheck;
    private bool _equals;

    public void Init(
        string tag,
        bool equals,
        OnEnter onEnter
        )
    {
        _tagToCheck = tag;
        _equals = equals;
        _onEnter = onEnter;
    }

    void OnTriggerEnter(Collider other)
    {
        if (DebugThis)
        {
            Debug.Log(
                "GO: " + other.name +
                "tag: " + other.tag
                );
        }

        if (_onEnter != null)
        {
            if (_equals && other.tag == _tagToCheck)
            {
                _onEnter(other.gameObject);
            }
            else if (!_equals && other.tag != _tagToCheck)
            {
                _onEnter(other.gameObject);
            }
        }
    }
}
