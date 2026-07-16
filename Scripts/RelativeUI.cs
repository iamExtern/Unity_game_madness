using UnityEngine;
using YG;

public class RelativeUI : MonoBehaviour
{
    private float defaultScale;
    private RectTransform rt;
    private bool initializeThis = false;

    public Vector2 relativeTarget;

    private void Update()
    {
        Change();
    }

    [ContextMenu("ApplyRelativePosition")]
    public void ApplyRelativePosition()
    {
        RectTransform _rt = GetComponent<RectTransform>();
        relativeTarget = new Vector2(MathExtra.FromLerp(0f, Screen.width, _rt.position.x), MathExtra.FromLerp(0f, Screen.height, _rt.position.y));
    }

    private void Change()
    {
        Vector2 newPos = new Vector2(relativeTarget.x * Screen.width, relativeTarget.y * Screen.height);
        rt.position = newPos;

        float ratio;
        if (Screen.height < Screen.width)
        {
            ratio = Screen.height / (float)Screen.width;
        }
        else
        {
            ratio = Screen.width / (float)Screen.height;
        }

        rt.localScale = MathExtra.SetVector3(defaultScale * (ratio / ScreenManager.maxScreenRatioVertical));
    }

    private void OnEnable()
    {
        if (!initializeThis)
        {
            rt = GetComponent<RectTransform>();
            defaultScale = rt.localScale.x;
            initializeThis = true;
        }

        Change();
    }
}
