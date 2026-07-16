using UnityEngine;

public class AdaptiveUI : MonoBehaviour
{
    private float defaultScale;
    private RectTransform rt;
    private bool firstEnable = true;
    private bool initializeThis = false;

    private void ChangeScale(float newRatio)
    {
        rt.localScale = MathExtra.SetVector3(newRatio / ScreenManager.maxScreenRatio * defaultScale);
    }

    private void NormalizeScale()
    {
        rt.localScale = MathExtra.SetVector3(defaultScale);
    }

    private void SetSuitableScale()
    {
        if (ScreenManager.currentRatio > ScreenManager.maxScreenRatio)
            NormalizeScale();
        else
            ChangeScale(ScreenManager.currentRatio);
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

        ScreenManager.onScreenRationChange += ChangeScale;
        ScreenManager.onScreenRationNormalize += NormalizeScale;
        firstEnable = false;
    }

    private void OnDisable()
    {
        ScreenManager.onScreenRationChange -= ChangeScale;
        ScreenManager.onScreenRationNormalize -= NormalizeScale;
    }
}
