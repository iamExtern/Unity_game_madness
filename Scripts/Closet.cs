using UnityEngine;

public class Closet : Nightstand
{
    public Transform doorR;
    public Transform doorL;
    public GameObject colliderClose;
    public GameObject colliderOpen;

    protected override void Start()
    {
        lerper = new Lerper(1f / 0.3f, Openner);
    }

    public override void Interactive()
    {
        base.Interactive();

        colliderClose.SetActive(!open);
        colliderOpen.SetActive(open);
    }

    protected override void Openner(float cof)
    {
        doorL.localEulerAngles = new Vector3(-180f, 0f, Mathf.Lerp(0f, -130f, cof));
        doorR.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, -130f, cof));
    }
}
