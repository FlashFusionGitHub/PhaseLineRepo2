using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TweenCurveHelper : MonoBehaviour {

    public enum CurveType
    {
        EaseLinear,
        EaseIn,
        EaseInStrong,
        EaseOut,
        EaseOutStrong,
        EaseInOut,
        EaseInOutStrong,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseSpring,
        EaseBounce,
        EaseWobble,
    };

    public static AnimationCurve GetCurve(CurveType ct)
    {
        switch (ct)
        {
            case CurveType.EaseLinear:               return Tween.EaseLinear;
            case CurveType.EaseIn:                   return Tween.EaseIn;
            case CurveType.EaseInStrong:             return Tween.EaseInStrong;
            case CurveType.EaseOut:                  return Tween.EaseOut;
            case CurveType.EaseOutStrong:            return Tween.EaseOutStrong;
            case CurveType.EaseInOut:                return Tween.EaseInOut;
            case CurveType.EaseInOutStrong:          return Tween.EaseInOutStrong;
            case CurveType.EaseInBack:               return Tween.EaseInBack;
            case CurveType.EaseOutBack:              return Tween.EaseOutBack;
            case CurveType.EaseInOutBack:            return Tween.EaseInOutBack;
            case CurveType.EaseSpring:               return Tween.EaseSpring;
            case CurveType.EaseBounce:               return Tween.EaseBounce;
            case CurveType.EaseWobble:               return Tween.EaseWobble;
            default: return null;
        }
    }

    
}
