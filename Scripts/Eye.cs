using UnityEngine;

public class Eye : MonoBehaviour
{
    public Transform camera;

    private Vector2 max = new Vector2(30f, 30f);
    private Vector2 min = new Vector2(330f, 330f);

    public void LookAtPlayer(float shakingForce)
    {
        transform.LookAt(camera);

        Vector2 limitRot = MathExtra.RotationLimitXY(transform.localEulerAngles, max, min);
        limitRot += MathExtra.RandomVector2(shakingForce);

        transform.localEulerAngles = new Vector3(limitRot.x, limitRot.y, transform.localEulerAngles.z);
    }

    public void ResetRotation()
    {
        transform.localEulerAngles = Vector3.zero;
    }
}
