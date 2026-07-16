using UnityEngine;

public class EmpClip
{
    private int[] clip;
    private float stepDelay;
    private float random;
    private bool useRandom = false;

    private bool randomLevel = false;
    private int minRandomLevel;
    private int maxRandomLevel;

    private int currentStep = 0;

    public EmpClip(int[] clip, float stepDelay)
    {
        this.clip = clip;
        this.stepDelay = stepDelay;
        useRandom = false;
    }

    public EmpClip(int[] clip, float stepDelay, float random) : this(clip, stepDelay)
    {
        this.random = random;
        useRandom = true;
    }

    public EmpClip(int minRandomLevel, int maxRandomLevel, float stepDelay, float random)
    {
        this.minRandomLevel = minRandomLevel;
        this.maxRandomLevel = maxRandomLevel;
        this.stepDelay = stepDelay;
        this.random = random;
        useRandom = true;
        randomLevel = true;
    }

    public int GetStep()
    {
        if (!randomLevel)
        {
            if (currentStep > clip.Length - 1)
                currentStep = 0;

            currentStep++;

            return clip[currentStep - 1];
        }
        else
        {
            return Random.Range(1, maxRandomLevel + 1);
        }
    }

    public void ResetStep()
    {
        currentStep = 0;
    }

    public float GetDelay()
    {
        if (!useRandom)
            return stepDelay;
        else
            return stepDelay + Random.Range(-stepDelay * random, stepDelay * random);
    }
}
