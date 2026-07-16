using UnityEngine;

public class CodeLockWheel : MonoBehaviour
{
    public float wheelSpinCorrect = 1f;
    public int number;

    private float startRot;
    private float mouseStartY;
    private Vector3 virtualRot = new Vector3(342f, -90f, -90f);

    public void Select()
    {
        startRot = virtualRot.x;
        mouseStartY = Input.mousePosition.y;
    }

    public void Rotation()
    {
        CalculateNewRot(out float newRotX, out float remainder);

        if (remainder > 7f && remainder < 29f)
        {
            virtualRot = new Vector3(newRotX, -90f, -90f);
            transform.localEulerAngles = virtualRot;
        }
        else
            SetNumber(newRotX, remainder);
    }

    public void ToNearestNumber()
    {
        CalculateNewRot(out float newRotX, out float remainder);
        SetNumber(newRotX, remainder);
    }

    private void CalculateNewRot(out float newRotX, out float remainder)
    {
        float mouseDeltaY = Input.mousePosition.y - mouseStartY;
        float _newRotX = startRot + (mouseDeltaY * wheelSpinCorrect);
        _newRotX = _newRotX % 360f;
        if (_newRotX < 0f)
            _newRotX += 360f;
        newRotX = _newRotX;
        remainder = (_newRotX - 18f) % 36f;
    }

    private void SetNumber(float newRotX, float remainder)
    {
        if (remainder > 18f)
            virtualRot = new Vector3(newRotX + (36f - remainder), -90f, -90f);
        else
            virtualRot = new Vector3(newRotX - remainder, -90f, -90f);

        transform.localEulerAngles = virtualRot;
        int newNumber = ((int)(360f - virtualRot.x + 18f) / 36) - 1;
        if (number != newNumber)
            SoundManager.instance.PlaySound(SoundManager.Sound.CodeLockWheel);
        number = newNumber;
    }
}
