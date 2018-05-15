using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public enum PlatformPosition
    {
        Up,
        Down
    }

    public PlatformPosition CurrentPlatformPosition;

    public List<float> BlocksPos;
    public List<Transform> Blocks;
    public List<float> BlocksTime;
    public List<int> BlocksEase;

    // Use this for initialization
    void Awake()
    {
        Blocks = new List<Transform>();
        BlocksPos = new List<float>();
        BlocksTime = new List<float>();
        BlocksEase = new List<int>();

        foreach (Transform item in transform)
        {
            if (item.name.Contains("Block."))
            {
                // - save amount to move the blocks up and down.
                float posValueMultiplier = 2.75f;
                if (item.name.Contains("("))
                {
                    string posValueMultiplierString = item.name.Split('(')[1];
                    posValueMultiplierString = posValueMultiplierString.Split(')')[0];
                    posValueMultiplier = Convert.ToSingle(posValueMultiplierString);
                }

                // save the stuff in the lists variables
                Blocks.Add(item);
                BlocksPos.Add(posValueMultiplier);
            }
        }

        Move(PlatformPosition.Down, snap: true, reset: true);
    }

    public void Move(PlatformPosition platformPosition, float timeMultiplier = 1f, bool snap = false, bool reset = false)
    {
        CurrentPlatformPosition = platformPosition;

        if (reset) {
            for (var i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].position = Vector3.zero;
            }
        }

        for (var i = 0; i < Blocks.Count; i++)
        {
            Vector3 posMultiplier;
            if (platformPosition == PlatformPosition.Up)
                posMultiplier = new Vector3(0, BlocksPos[i], 0);
            else
                posMultiplier = new Vector3(0, -BlocksPos[i], 0);

            var newPos = Blocks[i].position + posMultiplier;
            if (snap)
                Blocks[i].position = newPos;
            else
                LeanTween.move(Blocks[i].gameObject, newPos, BlocksTime[i] * timeMultiplier).setEase((LeanTweenType)BlocksEase[i]);
        }

        GenerateRandomMovement();
    }

    public float GetLastBlockTimeToPosition()
    {
        float maxTime = 0f;
        foreach (float val in BlocksTime)
        {
            if (maxTime < val)
                maxTime = val;
        }
        return maxTime;
    }

    /// <summary>
    /// caculate the random rate at which they move
    /// </summary>
    private void GenerateRandomMovement()
    {
        for (var i = 0; i < Blocks.Count; i++)
            BlocksTime.Add(UnityEngine.Random.Range(1.0f, 3.2f));
        for (var i = 0; i < Blocks.Count; i++)
            BlocksEase.Add(UnityEngine.Random.Range(1, 29));
    }
}
