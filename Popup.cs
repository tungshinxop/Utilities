using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Action onFinishOpen;
    public Action onStartClose;

    public PopupAnimation popUpAnimation;

    public GameObject window;
    public Image raycastBlock;
    
    public bool showBlockRayWithPopup;
    public float autoCloseTime = 3f;
    public float popupAnimationTime = 0.75f;
    public float yOffset = 1000;
    
    public bool destroyOnClose;
    public bool autoClose;
    public bool closeImmediately;
    
    protected float _defaultAlpha = 140;
    public virtual void OnEnable()
    {
        SetRaycastTransparency(_defaultAlpha);
        StartCoroutine(ShowPopup());
        if (autoClose)
        {
            StartCoroutine(AutoClose());
        }
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        if (gameObject.activeSelf)
        {
            onStartClose?.Invoke();
            Utils.SoundButtonClick();
            StartCoroutine(HidePopup());
        }
    }

    protected IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseTime);
        Close();
    }

    private IEnumerator ShowPopup()
    {
        Utils.SoundButtonClick();
        var animateTime = popupAnimationTime;

        SetRaycastTransparency(1);
        
        switch (popUpAnimation)
        {
            case PopupAnimation.None:
                animateTime = 0;
                break;
            case PopupAnimation.SlideUp:
                var bottom = window.transform.localPosition;
                bottom.y = -yOffset;
                window.transform.localPosition = bottom;
                window.transform.DOLocalMoveY(0, popupAnimationTime);
                break;
            case PopupAnimation.ScaleUp:
                window.transform.transform.localScale = Vector3.zero;
                window.transform.DOScale(Vector3.one, popupAnimationTime).SetEase(Ease.OutBounce);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        yield return new WaitForSeconds(animateTime);
        
        SetRaycastTransparency(_defaultAlpha);

        onFinishOpen?.Invoke();
    }

    private void SetRaycastTransparency(float alpha)
    {
        if (showBlockRayWithPopup)
        {
            var temp = raycastBlock.color;
            temp.a = alpha / 255f;
            raycastBlock.color = temp;
        }
    }
    
    protected IEnumerator HidePopup()
    {
        if (closeImmediately)
        {
            onHide();
            yield break;
        }
        
        var animateTime = popupAnimationTime;

        SetRaycastTransparency(1);
        
        switch (popUpAnimation)
        {
            case PopupAnimation.None:
                animateTime = 0;
                break;
            case PopupAnimation.SlideUp:
                window.transform.DOLocalMoveY(-yOffset, popupAnimationTime);
                break;
            case PopupAnimation.ScaleUp:
                window.transform.DOScale(Vector3.zero, popupAnimationTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        yield return new WaitForSeconds(animateTime);

        onHide();

        void onHide()
        {
            if (destroyOnClose)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}

public enum PopupAnimation
{
    None,
    SlideUp,
    ScaleUp
}
