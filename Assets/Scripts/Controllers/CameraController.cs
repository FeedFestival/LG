using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private IEnumerator _moveTowardsTheAction;
    private IEnumerator _moveTowardsTheStart;

    private float _startDuration;

    private float lastZ = 50f;
    private float _duration = 1f;

    private int _moveCameraTweenId;

    public void Init()
    {
        transform.position = new Vector3(-13, 20, 13);
        transform.eulerAngles = new Vector3(52, -226, 0);

        MoveTowardsTheStart();
    }

    // Use this for initialization
    public void StartFight()
    {
        //Game.Instance().UiController.HealthBarsPanel.SetActive(true);

        _moveTowardsTheAction = MoveTowardsTheAction();
        StartCoroutine(_moveTowardsTheAction);
    }

    private void MoveTowardsTheStart(bool stop = false)
    {
        if (stop) {
            LeanTween.cancel(_moveCameraTweenId);
            return;
        }

        LTDescr _moveProjectile = LeanTween.move(gameObject, new Vector3(-12, 20, 46), 50f).setEase(LeanTweenType.linear);
        _moveCameraTweenId = _moveProjectile.id;
    }

    private IEnumerator MoveTowardsTheAction()
    {
        yield return new WaitForSeconds(_duration);

        var x = transform.position.x;
        var y = transform.position.y;

        float maxZ = 100f; float minZ = -1f;
        int aliveUnits = 0;
        for (var i = 0; i < Fight.Instance().PlayerUnits.Count; i++)
        {
            if (Fight.Instance().PlayerUnits[i].Stats.IsDead == false)
                aliveUnits++;
        }
        if (aliveUnits > 1)
        {
            foreach (Unit unit in Fight.Instance().PlayerUnits)
            {
                if (unit.transform.position.z < maxZ)
                {
                    maxZ = unit.transform.position.z + 7f;
                }
                if (unit.transform.position.z > minZ || minZ < 0)
                {
                    minZ = unit.transform.position.z + 7f;
                }
            }
            var diff = maxZ - minZ;
            //Debug.Log(maxZ + " - " + minZ + " = " + diff);
            diff = diff / 2;
            maxZ = maxZ - diff;
            if (Mathf.Floor(lastZ) > Mathf.Floor(maxZ))
                lastZ = maxZ;

            var newPosition = new Vector3(x, y, lastZ);
            LeanTween.move(transform.gameObject, newPosition, _duration);
        }
        else
        {
            var z = Fight.Instance().PlayerUnits[0].gameObject.transform.position.z + 7;
            var newPosition = new Vector3(x, y, z);
            LeanTween.move(transform.gameObject, newPosition, _duration);
        }

        _moveTowardsTheAction = MoveTowardsTheAction();
        StartCoroutine(_moveTowardsTheAction);
    }
}
