using System;
using System.Collections;
using System.Collections.Generic;
using UIManager;
using UnityEngine;

public abstract class UITransition : MonoBehaviour
{
    private BaseUIScreen screen;

    public abstract bool isPlaying { get; }

    protected virtual void Awake()
    { 
        screen = GetComponentInParent<BaseUIScreen>();
    }

    protected virtual void OnEnable()
    {
        if (Application.isPlaying)
        {
            screen.uiTransitioners.Add(this);
        }
    }
    
    protected virtual void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (screen != null)
            {
                screen.uiTransitioners.Remove(this);
            }
        }
    }

    public abstract void PlayForward();
    public abstract void PlayBackwards();

}
