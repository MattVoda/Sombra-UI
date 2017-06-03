using UnityEngine;
using UnityEngine.Assertions;

public static class EasingFunctions
{
    public static float easeIn(float t, float e = 2f) {
        Assert.IsTrue(e >= 1f);
        return 1f - Mathf.Pow(1f - t, e);
    }

    public static float easeOut(float t, float e = 2f) {
        Assert.IsTrue(e >= 1f);
        return Mathf.Pow(t, e);
    }

    public static float easeInOut(float t, float e = 2f) {
        Assert.IsTrue(e >= 1f);
        return Mathf.Pow(t, e) / (Mathf.Pow(t, e) + Mathf.Pow(1f - t, e));
    }
}