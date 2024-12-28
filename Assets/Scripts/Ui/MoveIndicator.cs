using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    public Animator[] MoveIndicators;
    public AnimationClip AnimationClip;
    public float AnimationSpeed;
    public float AnimationLength;
    public bool IsPlaying;
    private IEnumerator _waitForAnimation;

    void Start()
    {
        AnimationLength = AnimationClip.length / AnimationSpeed;
        foreach (Animator anim in MoveIndicators)
        {
            anim.SetFloat("speed", AnimationSpeed);
        }
    }

    internal void Play()
    {
        if (IsPlaying)
        {
            StopCoroutine(_waitForAnimation);
            SetPlay(false);

            Timer._.InternalWait(() =>
            {
                SetPlay(true);
                _waitForAnimation = WaitForAnimation();
                StartCoroutine(_waitForAnimation);
            });

            return;
        }

        SetPlay(true);
        _waitForAnimation = WaitForAnimation();
        StartCoroutine(_waitForAnimation);
    }

    private void SetPlay(bool play)
    {
        foreach (Animator anim in MoveIndicators)
        {
            anim.SetBool("playMoveArrows", play);
        }
        IsPlaying = play;
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(AnimationLength);

        SetPlay(false);
    }
}
