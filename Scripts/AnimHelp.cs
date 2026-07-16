using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimHelp : MonoBehaviour
{
    public static Action<int> onAnimEnd;
    public Animator anim;

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    public void AnimEnd(int id)
    {
        onAnimEnd?.Invoke(id);
    }

    public void EmpNoBatterySound()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.NoBatterySound);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlaySoundKill()
    {
        if (Player.instance.monster.playerKilling)
        {
            SoundManager.instance.AuidoPause(true);
            SoundManager.instance.PlaySound(SoundManager.Sound.BadEnd);
        }
    }

    public void PlaySoundExit()
    {
        if (!Player.instance.monster.playerKilling)
        {
            SoundManager.instance.AuidoPause(true);
            SoundManager.instance.PlaySound(SoundManager.Sound.GoodEnd);
        }
    }
}
