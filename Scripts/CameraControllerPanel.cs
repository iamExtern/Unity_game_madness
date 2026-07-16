using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControllerPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static CameraControllerPanel instance = null;

    public bool pressed = false;
    private int fingerId;

    void Awake()
    {
        instance = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject && !Options.pc)
        {
            pressed = true;
            fingerId = eventData.pointerId;

            Player.instance.controller.TouchDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Options.pc)
        {
            pressed = false;
            Player.instance.controller.TouchUp();
        }
    }

    public Vector2 GetTouchDeltaPosition()
    {
        if (!Options.pc)
        {
            if (pressed)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.fingerId == fingerId)
                    {
                        if (touch.phase == TouchPhase.Moved)
                        {
                            return touch.deltaPosition;
                        }
                        if (touch.phase == TouchPhase.Stationary)
                        {
                            return Vector2.zero;
                        }
                    }
                }
            }
        }

        return Vector2.zero;
    }
}
