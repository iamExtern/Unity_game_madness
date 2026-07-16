using System;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public const float maxScreenRatio = 1920f / 1080f;
    public const float maxScreenRatioVertical = 1080f / 1920f;

    private float lastScreenRatio;
    private float lastScreenRatioVertical;
    private bool normalizeInvoke = false;

    private int lastWidth;
    private int lastHeight;

    public static float currentRatio;
    public static float currentRatioVertical;

    public static Action<float> onScreenRationChange;
    public static Action onScreenRationNormalize;

    public static Action<float> onScreenRationChangeVertical;
    public static Action onScreenRationNormalizeVertical;

    public static Action<float> onScreenChanged;

    public static Action onScreenSizeChanged;

    void Update()
    {
        currentRatio = Screen.width / (float)Screen.height;

        if (currentRatio != lastScreenRatio && currentRatio <= maxScreenRatio)
        {
            normalizeInvoke = false;
            onScreenRationChange?.Invoke(currentRatio);
        }
        else if (currentRatio > maxScreenRatio && !normalizeInvoke)
        {
            normalizeInvoke = true;
            onScreenRationNormalize?.Invoke();
        }

        currentRatioVertical = Screen.height / (float)Screen.width;

        if (currentRatioVertical != lastScreenRatio && currentRatioVertical <= maxScreenRatioVertical)
        {
            normalizeInvoke = false;
            onScreenRationChangeVertical?.Invoke(currentRatioVertical);
        }
        else if (currentRatioVertical < maxScreenRatioVertical && !normalizeInvoke)
        {
            normalizeInvoke = true;
            onScreenRationNormalizeVertical?.Invoke();
        }

        if (currentRatio != lastScreenRatio)
            onScreenChanged?.Invoke(currentRatio);

        if (Screen.width != lastWidth || Screen.height != lastHeight)
            onScreenSizeChanged?.Invoke();

        lastScreenRatio = currentRatio;
        lastScreenRatioVertical = currentRatioVertical;

        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}
