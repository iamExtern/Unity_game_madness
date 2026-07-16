using UnityEngine;

public class CameraEffector : MonoBehaviour
{
    public static CameraEffector instance = null;
    public float positonShakeCorrect;
    public float shakingForce = 0.3f;
    public Camera cam;
    public bool shaking = false;
    private bool init = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        Shaking(shakingForce);
    }

    public void Shaking(float shake)
    {
        if (!shaking)
            return;

        transform.localEulerAngles = MathExtra.RandomVector3(shake);
        transform.localPosition = new Vector3(MathExtra.Rand1(shake) * positonShakeCorrect, MathExtra.Rand1(shake) * positonShakeCorrect);
    }

    private void OnScreenRatioChange(float ratio = 0f)
    {
        cam.aspect = ((float)Screen.width) / Screen.height;
    }

    private void OnEnable()
    {
        OnScreenRatioChange();
        ScreenManager.onScreenChanged += OnScreenRatioChange;
    }

    private void OnDisable()
    {
        ScreenManager.onScreenChanged -= OnScreenRatioChange;
    }
}
