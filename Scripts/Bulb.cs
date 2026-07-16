using UnityEngine;

public class Bulb : MonoBehaviour
{
    public Material mat;

    private void Start()
    {
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            mat.SetFloat("_Light", 100f);
        }
        else
        {
            mat.SetFloat("_Light", 1f);
        }
    }
}
