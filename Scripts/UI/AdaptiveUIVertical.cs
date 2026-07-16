using UnityEngine;

public class AdaptiveUIVertical : MonoBehaviour
{
    private float defaultScale;
    private RectTransform rt;
    private bool firstEnable = true;
    private bool initializeThis = false;

    private void ChangeScale(float newRatio)
    {
        rt.localScale = MathExtra.SetVector3(newRatio / ScreenManager.maxScreenRatioVertical * defaultScale);
    }

    private void NormalizeScale()
    {
        rt.localScale = MathExtra.SetVector3(defaultScale);
    }

    private void SetSuitableScale()
    {
        if (ScreenManager.currentRatioVertical > ScreenManager.maxScreenRatioVertical)
            NormalizeScale();
        else
            ChangeScale(ScreenManager.currentRatioVertical);
    }

    private void OnEnable()
    {
        if (!initializeThis)
        {
            rt = GetComponent<RectTransform>();
            defaultScale = rt.localScale.x;
            initializeThis = true;
        }

        SetSuitableScale();

        ScreenManager.onScreenRationChangeVertical += ChangeScale;
        ScreenManager.onScreenRationNormalizeVertical += NormalizeScale;
        firstEnable = false;
    }

    private void OnDisable()
    {
        ScreenManager.onScreenRationChangeVertical -= ChangeScale;
        ScreenManager.onScreenRationNormalizeVertical -= NormalizeScale;
    }
}
