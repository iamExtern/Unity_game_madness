using UnityEngine;

public class SelfSound : MonoBehaviour
{
    public AudioSource source;
    public bool sourcePause = false;

    public void SourcePause(bool pause)
    {
        sourcePause = pause;

        if (pause)
        {
            source.Pause();
        }
        else if (!SoundManager.instance.audioPause)
        {
            source.UnPause();
        }
    }

    public void DontDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Pause(bool pause)
    {
        if (pause)
        {
            if (!sourcePause)
                source.Pause();
        }
        else
        {
            if (!sourcePause)
                source.UnPause();
        }

    }

    private void OnEnable()
    {
        SoundManager.onAudioPause += Pause;
    }

    private void OnDisable()
    {
        SoundManager.onAudioPause -= Pause;
    }
}
