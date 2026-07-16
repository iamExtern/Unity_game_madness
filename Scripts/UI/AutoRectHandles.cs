#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RectTransformExtensions
{
    [MenuItem("CONTEXT/RectTransform/Automatic Anchor")]
    public static void AutomaticAnchor(MenuCommand command)
    {
        RectTransform rt = (RectTransform)command.context;
        RectTransform parent_rt = rt.parent.GetComponent<RectTransform>();

        Vector2 positionOnParent = rt.localPosition;

        Vector2 minOnParent = positionOnParent + rt.rect.min;
        Vector2 maxOnParent = positionOnParent + rt.rect.max;

        Vector2 minAnchor = MathExtra.FromLerpVector2(parent_rt.rect.min, parent_rt.rect.max, minOnParent);
        Vector2 maxAnchor = MathExtra.FromLerpVector2(parent_rt.rect.min, parent_rt.rect.max, maxOnParent);

        rt.anchorMin = minAnchor;
        rt.anchorMax = maxAnchor;
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;
    }
}
#endif
