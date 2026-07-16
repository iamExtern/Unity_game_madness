using UnityEngine;

public class PausePanelProvider : MonoBehaviour
{
    public AsyncLoader loader;
    public void Pause()
    {
        PausePanel.instance.Pause();
    }

    public void Continue()
    {
        PausePanel.instance.Continue();
    }

    public void ToMainMenu()
    {
        PausePanel.instance.ToMainMenu();
    }

    public void SettingsOpen()
    {
        PausePanel.instance.SettingsOpen();
    }

    public void SettingsClose()
    {
        PausePanel.instance.SettingsClose();
    }

    public void ResetSettings()
    {
        PausePanel.instance.ResetSettings();
    }

    public void SetSens()
    {
        PausePanel.instance.SetSens();
    }

    public void SetVolume()
    {
        PausePanel.instance.SetVolume();
    }

    public void Play()
    {
        loader.LoadGameScene();
    }
}
