using System;
using System.Collections.Generic;

public static class Easing
{
    // GENERAL
    public static float Ease(float t, EaseType easeType) => EasingFunctions[easeType](t);

    // LINEAR
    public static float Linear(float t) => t;

    // IN (as t^x)
    public static float EaseInQuad(float t) => t * t;

    public static float EaseInCubic(float t) => t * t * t;

    public static float EaseInQuart(float t) => t * t * t * t;

    public static float EaseInQuint(float t) => t * t * t * t * t;

    // OUT (as (1-t)^x)
    public static float EaseOutQuad(float t)
    {
        var q = 1 - t;
        return 1 - q * q;
    }

    public static float EaseOutCubic(float t)
    {
        var q = 1 - t;
        return 1 - q * q * q;
    }

    public static float EaseOutQuart(float t)
    {
        var q = 1 - t;
        return 1 - q * q * q * q;
    }

    public static float EaseOutQuint(float t)
    {
        var q = 1 - t;
        return 1 - q * q * q * q;
    }

    // INOUT (as xfade between in and out)
    public static float EaseInOutQuad(float t) => CrossFade(EaseInQuad, EaseOutQuad, t);

    public static float EaseInOutCubic(float t) => CrossFade(EaseInCubic, EaseOutCubic, t);

    public static float EaseInOutQuart(float t) => CrossFade(EaseInQuart, EaseOutQuart, t);

    public static float EaseInOutQuint(float t) => CrossFade(EaseInQuint, EaseOutQuint, t);

    // MIX
    public static float Mix(Func<float, float> A, Func<float, float> B, float mix, float t)
        => (1 - mix) * A(t) + mix * B(t);

    // CrossFade
    public static float CrossFade(Func<float, float> A, Func<float, float> B, float t) => (1 - t) * A(t) + t * B(t);

    // dictionary to store the methods
    public static readonly Dictionary<EaseType, Func<float, float>> EasingFunctions =
        new Dictionary<EaseType, Func<float, float>>
        {
            {EaseType.Linear, Linear},
            {EaseType.InQuad, EaseInQuad},
            {EaseType.OutQuad, EaseOutQuad},
            {EaseType.InOutQuad, EaseInOutQuad},
            {EaseType.InCubic, EaseInCubic},
            {EaseType.OutCubic, EaseOutCubic},
            {EaseType.InOutCubic, EaseInOutCubic},
            {EaseType.InQuart, EaseInQuart},
            {EaseType.OutQuart, EaseOutQuart},
            {EaseType.InOutQuart, EaseInOutQuart},
            {EaseType.InQuint, EaseInQuint},
            {EaseType.OutQuint, EaseOutQuint},
            {EaseType.InOutQuint, EaseInOutQuint}
        };
}

// enum for selecting an ease type
public enum EaseType
{
    Linear,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint
}