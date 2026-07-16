using UnityEngine;

public class PostShader : SceneEffector
{
    public static PostShader instance = null;
    public Monster monster;
    public Material eye;
    public Material screen;
    public SelfSound lowNoise;

    public float eyeT = -1f;

    public float lowNoiseVolume = 0f;
    public float noiseAmount = 0f;
    public float glitchStrength = 0f;
    public Color color = Color.white;
    public float flicker = 1f;
    public float speed = 0f;

    private void Awake()
    {
        instance = this;
    }

    protected override void SetValues()
    {
        if (!changing)
            return;

        if (!monster.playerKilling)
            eye.SetFloat("_t", eyeT);

        screen.SetFloat("_noiseAmount", noiseAmount);
        screen.SetFloat("_glitchStrength", glitchStrength);
        screen.SetColor("_fillColor", color);
        screen.SetFloat("_flicker", flicker);
        screen.SetFloat("_speed", speed);
        lowNoise.source.volume = lowNoiseVolume;
    }

    protected override void ResetValues()
    {
        eye.SetFloat("_t", -1f);

        screen.SetFloat("_noiseAmount", 0f);
        screen.SetFloat("_glitchStrength", 0f);
        screen.SetColor("_fillColor", color);
        screen.SetFloat("_flicker", 1f);
        screen.SetFloat("_speed", 0f);
    }

    public void RedGlitchEnd()
    {
        SoundManager.instance.SetActiveAggressionNoise(false);
    }

    public void RedGlitchStart()
    {
        SoundManager.instance.SetActiveAggressionNoise(true);
    }
}