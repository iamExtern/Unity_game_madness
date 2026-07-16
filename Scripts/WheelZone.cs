using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WheelZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int id;

    public static Action<int> onDown;
    public static Action<int> onUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke(id);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke(id);
    }
}
