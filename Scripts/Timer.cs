using UnityEngine;

public class Timer
{
    public bool isEnabled { get; private set; }

    private Method method;
    private float currentTime;

    public Timer(Method method)
    {
        this.method = method;
    }

    public void Start(float duration)
    {
        currentTime = duration;
        isEnabled = true;

        MonsterEventer.timerIteration += Iteration;
    }

    public void Stop()
    {
        isEnabled = false;

        MonsterEventer.timerIteration -= Iteration;
    }

    public void Resetart(float duration)
    {
        Stop();
        Start(duration);
    }

    public void AddTime(float durationCorrect)
    {
        currentTime += durationCorrect;
    }

    private void Iteration()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            method();
            Stop();
        }
    }

    public delegate void Method();
}
