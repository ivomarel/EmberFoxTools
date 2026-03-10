using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UIManager;
using UnityEngine;

[RequireComponent(typeof(DOTweenAnimation))]
public class UITween : UITransition
{
    private DOTweenAnimation dgTween;

    public float delayBeforeTween;

    [Tooltip("Useful if you want a reversed tween to have some delay")]
    public float delayAfterTween;

    private Sequence dgTweenSequence;

    public UITweenPlayTrigger playTrigger = UITweenPlayTrigger.PushAndBackwardsPop;

    public override bool isPlaying => dgTweenSequence.IsPlaying();

    protected override void Awake()
    {
        base.Awake();
        if (dgTween == null)
        {
            dgTween = GetComponent<DOTweenAnimation>();
        }
        dgTween.autoKill = false;
        dgTween.autoPlay = false;

        dgTweenSequence = DOTween.Sequence();
        dgTweenSequence.SetAutoKill(false);
        dgTweenSequence.AppendInterval(delayBeforeTween);
        //We use a manual update so when we delete an object with a Tween we avoid a null reference
        dgTweenSequence.SetUpdate(UpdateType.Manual);
        if (dgTween.tween == null)
        {
            Debug.LogWarning($"Tween on {gameObject.name} is ignored since no tween was found.", this);
        }
        else
        {
            dgTweenSequence.Append(dgTween.tween);
        }
        dgTweenSequence.AppendInterval(delayAfterTween);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying)
        {
            if (playTrigger != UITweenPlayTrigger.PushAndBackwardsPop)
            {
                dgTweenSequence.Restart();
            }
        }
    }

    public override void PlayForward()
    {
        if (playTrigger is UITweenPlayTrigger.PushAndBackwardsPop or UITweenPlayTrigger.Push)
        {
            dgTweenSequence.PlayForward();
        }
    }

    public override void PlayBackwards()
    {
        if (playTrigger is UITweenPlayTrigger.PushAndBackwardsPop)
        {
            dgTweenSequence.PlayBackwards();
        }
        if (playTrigger is UITweenPlayTrigger.Pop)
        {
            dgTweenSequence.PlayForward();
        }
    }

    private void Update()
    {
        dgTweenSequence.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }
}


public enum UITweenPlayTrigger
{
    PushAndBackwardsPop,
    Push,
    Pop,

    //Leave,
    //Return,
    Manual
}