using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class FightUtils
    {
        public delegate Projectile CreateProjectile(Projectile go);

        public static bool StopAttacking(IUnit unit)
        {
            if ((unit.UnitInteligence.UnitPrimaryState != UnitPrimaryState.Busy
                   || unit.UnitInteligence.UnitActionState != UnitActionState.Attacking
                   || unit.Stats.IsDead)
                || (unit.UnitInteligence.UnitPrimaryState == UnitPrimaryState.Stunned))
                return true;
            return false;
        }

        internal static Projectile GetAvailableProjectile(
            List<Projectile> gameObjects,
            CreateProjectile createProjectile,
            Unit.OnHit onHit,
            IAm iAm,
            ref int? projectileIndex
            )
        {
            Projectile go = null;
            int i = 0;
            foreach (Projectile obj in gameObjects)
            {
                if (obj.Available == true)
                {
                    go = obj;
                    projectileIndex = i;
                    //Debug.Log(obj.gameObject.activeSelf + " :" + DateTime.Now);
                    break;
                }
                i++;
            }

            if (go == null)
            {
                HitArea arrowHitArea = null;
                go = createProjectile(go);
                foreach (Transform hA in go.transform)
                {
                    arrowHitArea = hA.GetComponent<HitArea>();
                    if (arrowHitArea != null)
                        break;
                }
                if (arrowHitArea != null)
                    arrowHitArea.Init(onHit, iAm.ToString(), isProjectile: true, projectile: go);

                return go;
            }
            else
            {
                return null;
            }
        }

        internal static IAm OppositeTeam(IAm iam)
        {
            return iam == IAm.Ally ? IAm.Enemy : IAm.Ally;
        }

        public static float CalculateAtackSpeed(
            ref Animation animation,
            string reload, string attack,
            int attackSpeed,
            bool debug = false
            )
        {
            var reloadLength = AnimationUtils.GetAnimationLength(animation, reload);
            var attackLength = AnimationUtils.GetAnimationLength(animation, attack);

            if (debug)
                Debug.Log(
                    "as: " + attackSpeed + Environment.NewLine +
                    "asF: " + (float)attackSpeed + Environment.NewLine +
                    "cASf" + ((float)attackSpeed / 100)
                    );

            float aS = ((float)attackSpeed / 100);
            float fullCurrentTime = reloadLength + attackLength;

            if (debug)
                Debug.Log("fullCurrentTime: " + fullCurrentTime);
            float coeficient = (float)Math.Pow(1f / aS, 2);
            if (debug)
                Debug.Log("coeficient: " + coeficient + ", attackSpeed: " + aS);
            float desiredTime = coeficient * aS;
            if (debug)
                Debug.Log("desiredTime: " + desiredTime);
            float newSpeed = fullCurrentTime / desiredTime;

            if (debug)
                Debug.Log("newDeterminedAnimationSpeed: " + newSpeed);

            animation[reload].speed = newSpeed;
            animation[attack].speed = newSpeed;

            return newSpeed;
        }

        public static int GetDefaultAttackSpeed(
            Animation animation,
            string reload, string attack
            )
        {
            var reloadLength = AnimationUtils.GetAnimationLength(animation, reload);
            var attackLength = AnimationUtils.GetAnimationLength(animation, attack);

            float fullCurrentTime = reloadLength + attackLength;

            var defaultAttackSpeed = 1 / fullCurrentTime;
            return (int)(defaultAttackSpeed * 100);
        }

        public static void FaceEnemy(Vector3 enemyPosition, Transform t, bool onlyY = true,float time = 1f)
        {
            Vector3 dir = (enemyPosition - t.position);
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            Vector3 eulerAngles;
            if (onlyY)
                eulerAngles = new Vector3(t.position.x, rot.eulerAngles.y, t.position.z);
            else
                eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
            LeanTween.rotate(t.gameObject, eulerAngles, time);
        }

        //public bool IsFacingObject()
        //{
        //    var targetEnemy = Game.Instance().GetUnit(_targetEnemyTeam, _targetEnemyIndex.Value);

        //    // Check if the gaze is looking at the front side of the object
        //    Vector3 forward = transform.forward;
        //    Vector3 toOther = (targetEnemy.transform.position - transform.position).normalized;
        //    float dot = Vector3.Dot(forward, toOther);
        //    Debug.Log(dot);
        //    if (dot < 1.1f)
        //        return false;
        //    return true;
        //}
    }
}
