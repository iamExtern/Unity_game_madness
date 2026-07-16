using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static Action<bool> onAudioPause;
    public static SoundManager instance = null;

    public GameObject[] sounds;

    public SelfSound backgroundSound = null;
    public SelfSound empBeeping = null;
    public SelfSound aggressionNoise = null;

    public bool audioPause = false;

    private float[] empPitch = new float[] { 1f, 1.2f, 1.5f, 2f, 2.5f };

    private void Awake()
    {
        if (instance == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            instance = this;
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        empBeeping = PlayLoopSound(Sound.EmpBeeping);
        aggressionNoise = PlayLoopSound(Sound.AggressionNoise);

        empBeeping.DontDestroy();
        aggressionNoise.DontDestroy();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        empBeeping.SourcePause(true);
        aggressionNoise.SourcePause(true);

        if (scene.buildIndex == 1)
        {
            backgroundSound = PlayLoopSound(Sound.BackgroundSound);
            PostShader.instance.lowNoise = aggressionNoise;
        }
    }

    private SelfSound PlayLoopSound(Sound sound)
    {
        return Instantiate(sounds[(int)sound]).GetComponent<SelfSound>();
    }

    public void PlaySound(Sound sound)
    {
        Instantiate(sounds[(int)sound]);
    }

    public void PlaySound3D(Sound sound, Vector3 position)
    {
        Instantiate(sounds[(int)sound]).transform.position = position;
    }

    public void PlaySound3D(Sound sound, Vector3 position, Transform parent)
    {
        Transform clone = Instantiate(sounds[(int)sound]).transform;
        clone.position = position;
        clone.SetParent(parent);
    }

    public void SetEmpLevel(int level)
    {
        if (level == 0)
        {
            if (!empBeeping.sourcePause)
                empBeeping.SourcePause(true);
        }
        else
        {
            if (empBeeping.sourcePause)
                empBeeping.SourcePause(false);
            empBeeping.source.pitch = empPitch[level - 1];
        }
    }

    public void SetActiveAggressionNoise(bool active)
    {
        if (active)
        {
            if (aggressionNoise.sourcePause)
                aggressionNoise.SourcePause(false);
        }
        else
        {
            if (!aggressionNoise.sourcePause)
                aggressionNoise.SourcePause(true);
        }
    }

    public void RandomMonsterStep(Vector3 position)
    {
        Instantiate(sounds[UnityEngine.Random.Range(16, 20)]).transform.position = position;
    }

    public void RandomPlayerStep()
    {
        Instantiate(sounds[UnityEngine.Random.Range(24, 29)]);
    }

    public void ButtonSound()
    {
        PlaySound(Sound.UIButton);
    }

    public void AuidoPause(bool pause)
    {
        audioPause = pause;
        onAudioPause?.Invoke(pause);
    }

    public enum Sound
    {
        AggressionNoise,
        Ambient1,
        Ambient2,
        Ambient3,
        BackgroundSound,
        CodeLockWheel,
        DoorOpen,
        EmpBeeping,
        EmpCharge,
        EmpEnabled,
        EmpTake,
        KeyTake,
        LockDoor,
        LowNoiseShort,
        MonsterAggression,
        MonsterAggression2,
        MonsterStep1,
        MonsterStep2,
        MonsterStep3,
        MonsterStep4,
        NoBatterySound,
        NoteTake,
        NoteViewerOpen,
        NoteViewerTurning,
        PlayerStep1,
        PlayerStep2,
        PlaterStep3,
        PlayerStep4,
        PlayerStep5,
        UIButton,
        MonsterAggression3,
        MonsterAggression4,
        MonsterAggression5,
        MonsterAggression6,
        EyeDrawingManifest,
        GoodEnd,
        BadEnd
    }
}
