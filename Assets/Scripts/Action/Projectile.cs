using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileTrajectory
    {
        Line, Arc
    }

    private IEnumerator _whileFire;

    private ProjectileTrajectory _projectileTrajectory;

    private MeshRenderer _meshRenderer;

    public bool Available = true;

    private Vector3 _arrowObjPos;
    private Vector3 _arrowObjRot;

    public float MaxProjectileSpeed = 1.0f;
    private float _attackRange;

    private Transform _parentOrigin;
    private Vector3 _targetPosition;

    public List<Vector3> BezzierPath = new List<Vector3>()
    {
        new Vector3(0f, 0f, 0f),           // startPoint
        new Vector3(0f, 2.5f, 0f),           // endControl
        new Vector3(0f, 0.25f, 0f),           // startControl
        new Vector3(0f, 1.25f, 0f)         // endPoint
    };

    int _moveProjectileTweenId;
    int _rotateProjectileTweenId;

    // Use this for initialization
    public void Init(Vector3 arrowObjPos, Vector3 arrowObjRot, float attackRange, Transform parentOrigin, ProjectileTrajectory trajectory = ProjectileTrajectory.Line)
    {
        _arrowObjPos = arrowObjPos;
        _arrowObjRot = arrowObjRot;
        _attackRange = attackRange;
        _parentOrigin = parentOrigin;

        _projectileTrajectory = trajectory;

        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Available = true;
        PrepareArrow(true);
    }

    internal void Fire(Vector3 pos, Vector3 unitRotation)
    {
        _targetPosition = pos;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, unitRotation.y, transform.eulerAngles.z);

        if (_projectileTrajectory == ProjectileTrajectory.Arc)
        {
            LTBezierPath path = CalculatePath(_targetPosition);
            float projectileTime = CalculateProjectileSpeed(_targetPosition);

            LTDescr _moveProjectile = LeanTween.move(gameObject, path, projectileTime).setEase(LeanTweenType.easeOutBack);
            _moveProjectileTweenId = _moveProjectile.id;

            LTDescr _rotateProjectile = LeanTween.rotateAroundLocal(gameObject, Vector3.left, 180f, projectileTime);
            _rotateProjectileTweenId = _rotateProjectile.id;

            _whileFire = WhileFireFire(projectileTime + UsefullUtils.GetPercent(projectileTime, 50f));
            StartCoroutine(_whileFire);
        }
    }

    private IEnumerator WhileFireFire(float time)
    {
        Available = false;
        yield return new WaitForSeconds(time);
        AfterFire();
    }

    public void AfterFire()
    {
        Available = true;
        StopCoroutine(_whileFire);
        _whileFire = null;
        PrepareArrow();
        _meshRenderer.enabled = false;

        LeanTween.cancel(_moveProjectileTweenId);
        LeanTween.cancel(_rotateProjectileTweenId);
    }

    private float CalculateProjectileSpeed(Vector3 pos)
    {
        float distance = Vector3.Distance(pos, transform.position);
        float percent = UsefullUtils.GetValuePercent(distance, _attackRange);
        float speed = UsefullUtils.GetPercent(MaxProjectileSpeed, percent);
        return speed;
    }

    private void PrepareArrow(bool setActive = false)
    {
        if (setActive)
            _meshRenderer.enabled = true;
        gameObject.transform.SetParent(null);
        var worldPosition = _parentOrigin.TransformPoint(_arrowObjPos);
        gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        gameObject.transform.localPosition = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        gameObject.transform.localEulerAngles = _arrowObjRot;

        //FightUtils.FaceEnemy(_targetPosition, transform, onlyY: false, time: 0.01f);
        //transform.LookAt(_targetPosition);
    }

    private LTBezierPath CalculatePath(Vector3 pos)
    {
        _meshRenderer.enabled = true;

        // It goes in the order: startPoint,endControl,startControl,endPoint - Note: the control for the end and start are reversed! This is just a quirk of the API.
        var relativePath = new Vector3[] {
                        gameObject.transform.position + BezzierPath[0],           // startPoint
                        pos + BezzierPath[1],                                              // endControl
                        gameObject.transform.position + BezzierPath[2],           // startControl
                        pos + BezzierPath[3]                                            // endPoint
            };
        return new LTBezierPath(
            relativePath
            );
    }
}
