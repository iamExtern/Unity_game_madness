using UnityEngine;

public class SceneLightProvider : SceneEffector
{
    public static SceneLightProvider instance = null;

    public Light[] lights;
    public Material lampLightColor;

    [ColorUsage(true, true)]
    public Color lampColor;

    [ColorUsage(true, true)]
    public Color defaultLampColor;

    public float directionalLightT;
    public float directionalLightPow = 1f;

    private const float defaultLightIntensity = 0.48f;

    private void Awake()
    {
        instance = this;
    }

    protected override void ResetValues()
    {
        lampLightColor.SetColor("_EmissionColor", defaultLampColor);
        lampLightColor.EnableKeyword("_EMISSION");

        for (int i = 0; i < lights.Length; i++)
            lights[i].intensity = defaultLightIntensity;
    }

    protected override void SetValues()
    {
        lampLightColor.SetColor("_EmissionColor", lampColor);

        for (int i = 0; i < lights.Length; i++)
            lights[i].intensity = defaultLightIntensity * Mathf.Pow(directionalLightT, directionalLightPow);
    }
}
