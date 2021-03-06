﻿using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.utils
{
    public static class __utils
    {
#pragma warning disable 0414 // private field assigned but not used.
        public static readonly string _version = "1.0.3";
#pragma warning restore 0414 //
        public static string ConvertNumberToK(int num)
        {
            if (num >= 1000)
                return string.Concat(num / 1000, "k");
            else
                return num.ToString();
        }

        public static Color SetColorAlpha(Color color, int value)
        {
            Color tempColor = color;
            tempColor.a = GetAlphaValue(value);
            return tempColor;
        }

        public static float GetAlphaValue(int value)
        {
            var perc = __percent.What(value, 255);
            return perc * 0.01f;
        }

        public static int GetRGBAAlphaValue(float value)
        {
            float perc = value * 100;
            return (int)__percent.Find(perc, 255);
        }

        public static void AddIfNone(int value, ref List<int> array, string debugAdd = null)
        {
            if (array.Contains(value))
            {
                return;
            }
            array.Add(value);
            if (string.IsNullOrEmpty(debugAdd) == false)
            {
                Debug.Log(debugAdd);
            }
        }

        public static int CreateLayerMask(bool aExclude, params int[] aLayers)
        {
            int v = 0;
            foreach (var L in aLayers)
                v |= 1 << L;
            if (aExclude)
                v = ~v;
            return v;
        }
    }

    public static class __percent
    {
        public static float Find(float _percent, float _of)
        {
            return (_of / 100f) * _percent;
        }
        public static float What(float _is, float _of)
        {
            return (_is * 100f) / _of;
        }

        public static int PennyToss(int _from = 0, int _to = 100)
        {
            var randomNumber = Random.Range(_from, _to);
            return (randomNumber > 50) ? 1 : 0;
        }

        public static T GetRandomFromArray<T>(T[] list)
        {
            List<int> percentages = new List<int>();
            int splitPercentages = Mathf.FloorToInt(100 / list.Length);
            int remainder = 100 - (splitPercentages * list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                int percent = i == (list.Length - 1) ? splitPercentages + remainder : splitPercentages;
                percentages.Add(percent);
            }
            for (int i = 1; i < percentages.Count; i++)
            {
                percentages[i] = percentages[i - 1] + percentages[i];
            }
            int randomNumber = UnityEngine.Random.Range(0, 100);
            int index = percentages.FindIndex(p => randomNumber < p);
            percentages = null;
            return list[index];
        }

        public static T GetRandomFromList<T>(List<T> list)
        {
            List<int> percentages = new List<int>();
            int splitPercentages = Mathf.FloorToInt(100 / list.Count);
            int remainder = 100 - (splitPercentages * list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                int percent = i == (list.Count - 1) ? splitPercentages + remainder : splitPercentages;
                percentages.Add(percent);
            }
            for (int i = 1; i < percentages.Count; i++)
            {
                percentages[i] = percentages[i - 1] + percentages[i];
            }
            int randomNumber = UnityEngine.Random.Range(0, 100);
            int index = percentages.FindIndex(p => randomNumber < p);
            percentages = null;
            return list[index];
        }
    }

    public static class world2d
    {
        public static Vector2 GetNormalizedDirection(Vector2 lastVelocity, Vector2 collisionNormal)
        {
            return Vector2.Reflect(lastVelocity.normalized, collisionNormal).normalized;
        }

        public static Vector3 LookRotation2D(Vector2 from, Vector2 to, bool fromFront = false)
        {
            Vector2 vectorToTarget = to - from;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            if (fromFront)
            {
                return new Vector3(q.eulerAngles.x, q.eulerAngles.y, q.eulerAngles.z - 90);
            }
            return q.eulerAngles;
        }
    }
}
