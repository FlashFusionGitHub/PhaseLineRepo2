using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Pixelplacement;

// a helper class that toggles a component between two positions (in and out) with a tween
// while respecting the objects anchors
public class TweenAnimator : MonoBehaviour {

    // go from here
    public Vector3 inPos;
    // to here
    public Vector3 outPos;
    // using this type of tween curve
    public TweenCurveHelper.CurveType curveType;
    // and take this long
    public float duration;

    public float inScale = 1;
    public float outScale = 1;

    // are we currently in or out?
    public bool isIn = true;

    // events called on start and end of the tween
    public UnityEvent onTweenBegin;
    public UnityEvent onTweenEnd;

    // toggles between in and out positions
    public void ToggleInOut()
    {
        Vector3 targetPos = isIn ? outPos : inPos;
        if (inPos != outPos)
            Tween.AnchoredPosition(transform as RectTransform, targetPos, duration, 0, TweenCurveHelper.GetCurve(curveType), Tween.LoopType.None, null, OnTweenEnd, false);

        float scale = isIn ? outScale : inScale;
        if (inScale != outScale)
            Tween.LocalScale(transform, scale * Vector3.one, duration, 0, TweenCurveHelper.GetCurve(curveType), Tween.LoopType.None, null, OnTweenEnd, false);
        onTweenBegin.Invoke();

        isIn = !isIn;
    }

    // call from events or script to move to the in position always
    public void TweenToInPos()
    {
        isIn = false;
        ToggleInOut();
    }


    // call from events or script to move to the out position always
    public void TweenToOutPos()
    {
        isIn = true;
        ToggleInOut();
    }

    // called from AnchoredPosition call above when tween ends
    void OnTweenEnd()
    {
        onTweenEnd.Invoke();
    }

    // helpers for use in the editor
    [ContextMenu("Set In Position")]
    public void SetInPos()
    {
        inPos = (transform as RectTransform).anchoredPosition;
    }

    [ContextMenu("Set Out Position")]
    public void SetOutPos()
    {
        outPos = (transform as RectTransform).anchoredPosition;
    }

    [ContextMenu("Go To In Position")]
    public void GoToInPos()
    {
        (transform as RectTransform).anchoredPosition = inPos;
    }

    [ContextMenu("Go To Out Position")]
    public void GoToOutPos()
    {
        (transform as RectTransform).anchoredPosition = outPos;
    }
}
