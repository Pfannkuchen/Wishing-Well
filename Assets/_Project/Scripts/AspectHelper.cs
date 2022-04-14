using UnityEngine;

public static class AspectHelper
{
    public static Vector2 GetPixelScale(float targetScale, Vector2 input)
    {
        return input * targetScale / (input.x > input.y ? input.x : input.y);
    }

    public static Vector2 GetNormalizedScale(float targetScale, Vector2 input)
    {
        return input * targetScale / (input.x > input.y ? input.x : input.y) / targetScale;
    }
}