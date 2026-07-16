using UnityEngine;

public class ButtonSoundProvider : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.instance.ButtonSound();
    }
}
