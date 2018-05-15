using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    public bool DebugThis;

    Projectile _projectile;

    Unit.OnHit _onHit;
    private string _tagToCheck;
    private bool _isProjectile;
    private List<int> _targets;

    public void Init(
        Unit.OnHit onHit
        , string tagToCheck
        //, bool singleTarget = false
        , bool isProjectile = false
        , Projectile projectile = null
        )
    {
        _onHit = onHit;
        _projectile = projectile;
        _tagToCheck = tagToCheck;
        _isProjectile = isProjectile;
    }

    void OnTriggerEnter(Collider other)
    {
        if (DebugThis)
            Debug.Log(
                "GO: " + other.name +
                "tag: " + other.tag
                );

        if (_onHit != null)
            if (other.tag == _tagToCheck)
            {
                if (_targets == null)
                    _targets = new List<int>();

                var targetIndex = other.GetComponent<IUnit>().Index;

                if (_targets.Contains(targetIndex))
                    return;

                _targets.Add(targetIndex);
                _onHit();

                if (_isProjectile)
                {
                    Reset();
                    if (transform.parent != null)
                    {
                        if (_projectile != null)
                            _projectile.AfterFire();
                        else
                            transform.parent.gameObject.SetActive(false);
                    }
                    else
                        transform.gameObject.SetActive(false);
                }
            }
    }

    public void Reset()
    {
        _targets = null;
    }
}
