using System.Collections;
using System.Collections.Generic;
using UIManager;
using UnityEngine;

public class BaseUIScreen : MonoBehaviour
{
    public bool hideCurrent = true;
    public bool duplicateOnReShow = false;
    public int priority = 0;

    private CanvasManager _canvasManager;
    [HideInInspector]
    public List<UITransition> uiTransitioners = new();

    protected bool isShowing;

    public CanvasManager canvasManager
    {
        get
        {
            if (_canvasManager == null)
                _canvasManager = this.GetComponentInParent<CanvasManager>();
            return _canvasManager;
        }
    }

    public virtual void OnShow()
    {
        gameObject.SetActive(true);
        isShowing = true;
        StartCoroutine(OnPushCoroutine());
    }

    private IEnumerator OnPushCoroutine()
    {
        foreach (UITransition transitioner in uiTransitioners)
        {
            transitioner.PlayForward();
        }

        if (IsAnyTransitionPlaying())
        {
            yield return StartCoroutine(WaitForTweensToFinish());
        }

        //We no longer complete anything if a transition was interrupted
        if (!isShowing)
            yield break;
        
        OnShowTransitionCompleted();
    }

    protected virtual void OnShowTransitionCompleted()
    {
    }

    private IEnumerator WaitForTweensToFinish()
    {
        bool state = isShowing;
        //Allow for all transitioners to register
        yield return null;
        foreach (UITransition transition in uiTransitioners)
        {
            while (transition.isPlaying)
            {
                if (state != isShowing)
                    yield break;
                
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private bool IsAnyTransitionPlaying()
    {
        foreach (UITransition transition in uiTransitioners)
        {
            if (transition.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    public virtual void OnHide()
    {
        canvasManager.RemoveVisible(this);
        isShowing = false;
        StartCoroutine(OnPopCoroutine());
    }

    private IEnumerator OnPopCoroutine()
    {
        foreach (var tween in uiTransitioners)
        {
            tween.PlayBackwards();
        }

        if (IsAnyTransitionPlaying())
        {
            yield return StartCoroutine(WaitForTweensToFinish());
        }

        //We no longer complete anything if a transition was interrupted
        if (isShowing)
            yield break;

        OnHideTransitionCompleted();
        Disable();
    }

    protected virtual void OnHideTransitionCompleted()
    {
    }


    private void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
