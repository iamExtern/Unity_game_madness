using UnityEngine;

public class MouthRope : MonoBehaviour
{
    public Transform cam;
    public Transform targetPoint;
    public Transform maxPoint;
    public Transform headTopRig;
    public Transform head;
    public Transform defaultHeadPoint;

    private float ropeLength;
    private float defaultScale;
    private float max = 60f;
    private float min = 330f;

    public void Init()
    {
        ropeLength = Vector3.Distance(maxPoint.position, transform.position);
        defaultScale = transform.localScale.z;
    }

    public void SetDistance(float distance, float shaking)
    {
        float newScaleZ = Vector3.Distance(transform.position, targetPoint.position) / ropeLength * defaultScale;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newScaleZ);
        headTopRig.position = defaultHeadPoint.position + (defaultHeadPoint.up * distance);
    }

    public void HeadLookAtPlayer(float shaking)
    {
        head.LookAt(cam);
        float newX = MathExtra.RotationLimitX(head.eulerAngles.x, max, min);
        head.eulerAngles = new Vector3(newX, head.eulerAngles.y, 0f);
        head.eulerAngles += MathExtra.RandomVector3(shaking);
    }

    public void ResetHeadRotation()
    {
        head.localEulerAngles = Vector3.zero;
    }
}
