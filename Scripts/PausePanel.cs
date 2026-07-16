using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class PausePanel : MonoBehaviour
{
    public static PausePanel instance = null;

    public static bool mainMenuLocalize = false;

    public bool pause = false;
    private bool lastCamShake;

    private const float maxSens = 2000f;

    private const float defaultVolume = 0.75f;
    private const float defaultSens = 350f;

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
    }

    private void Update()
    {
        PanelOpener();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSlidersStates();
        MainMenuLocalization();
    }

    private void RefreshSlidersStates()
    {
        if (PlayerPrefs.GetInt("FirstEntry") == 0)
        {
            PlayerPrefs.SetInt("FirstEntry", 1);

            PlayerPrefs.SetFloat("Volume", defaultVolume);
            PlayerPrefs.SetFloat("Sens", defaultSens);

            AudioListener.volume = defaultVolume;

            if (SceneManager.GetActiveScene().buildIndex == 1)
                Player.instance.controller.sens = defaultSens;
        }
        else
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");

            if (SceneManager.GetActiveScene().buildIndex == 1)
                Player.instance.controller.sens = PlayerPrefs.GetFloat("Sens");
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SettingsField.gameSens.SetValue(0f, maxSens, Player.instance.controller.sens);
            SettingsField.gameVolume.SetValue(0f, 1f, AudioListener.volume);
        }
        else
        {
            PausePanelMenuProvider.instance.mm_sens.SetValue(0f, maxSens, PlayerPrefs.GetFloat("Sens"));
            PausePanelMenuProvider.instance.mm_volume.SetValue(0f, 1f, AudioListener.volume);
        }
    }

    public void MainMenuLocalization()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            return;

        if (!YandexSDKConnect.invokingGetData)
            return;

        if (!mainMenuLocalize)
        {
            mainMenuLocalize = true;


            PausePanelMenuProvider.instance.playText.text = MultiLanguage.GetText(23);
            PausePanelMenuProvider.instance.settingsText.text = MultiLanguage.GetText(19);
            PausePanelMenuProvider.instance.volumeText.text = MultiLanguage.GetText(14);
            PausePanelMenuProvider.instance.sensText.text = MultiLanguage.GetText(15);
            PausePanelMenuProvider.instance.backText.text = MultiLanguage.GetText(16);
            PausePanelMenuProvider.instance.resetText.text = MultiLanguage.GetText(17);
            PausePanelMenuProvider.instance.loadingText.text = MultiLanguage.GetText(24);
        }
    }

    private void PanelOpener()
    {
        if (!Options.pc)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (SceneManager.GetActiveScene().buildIndex != 1)
                return;

            if (pause)
                Continue();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (StateProvider.playerInExitZone || Player.instance.monster.playerKilling)
            return;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!Options.pc && !NoteViewer.instance.open && !StateProvider.codeLockOpen)
                GuiManager.instance.SetActiveControllUI(false);
        }

        lastCamShake = CameraEffector.instance.shaking;
        CameraEffector.instance.shaking = false;

        pause = true;

        if (!NoteViewer.instance.open && !StateProvider.codeLockOpen)
        {
            Time.timeScale = 0f;
            SoundManager.instance.AuidoPause(true);
        }

        GuiManager.instance.pauseCont.SetActive(true);
        GuiManager.instance.settingsCont.SetActive(false);
        GuiManager.instance.pauseButtonsCont.SetActive(true);

        GuiManager.instance.LockHideAim(true);
        Player.instance.controller.SetControllerMode(Controller.ControllerMode.CursorControll);
    }

    public void UnPause()
    {
        if (pause)
        {
            pause = false;
            Time.timeScale = 1f;
            SoundManager.instance.AuidoPause(false);
        }
    }

    public void FreezeGame(bool freeze)
    {
        if (freeze)
        {
            Time.timeScale = 0f;
            SoundManager.instance.AuidoPause(true);
        }
        else
        {
            Time.timeScale = 1f;
            SoundManager.instance.AuidoPause(false);
        }
    }

    public void Continue()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!Options.pc && !NoteViewer.instance.open && !StateProvider.codeLockOpen)
                GuiManager.instance.SetActiveControllUI(true);
        }

        CameraEffector.instance.shaking = lastCamShake;
        pause = false;

        if (!NoteViewer.instance.open && !StateProvider.codeLockOpen)
        {
            Time.timeScale = 1f;
            SoundManager.instance.AuidoPause(false);
        }

        GuiManager.instance.pauseCont.SetActive(false);

        if (!NoteViewer.instance.open && !StateProvider.codeLockOpen)
        {
            Player.instance.controller.SetControllerMode(Controller.ControllerMode.Moving);
            GuiManager.instance.LockHideAim(false);
        }
    }

    public void ToMainMenu()
    {
        FreezeGame(true);
        YandexGame.FullscreenShow();
        SceneManager.LoadScene(0);
    }

    public void SettingsOpen()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GuiManager.instance.settingsCont.SetActive(true);
            GuiManager.instance.pauseButtonsCont.SetActive(false);
        }
        else
        {
            PausePanelMenuProvider.instance.mm_settingsCont.SetActive(true);
            PausePanelMenuProvider.instance.mm_buttonsCont.SetActive(false);
        }
    }

    public void SettingsClose()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GuiManager.instance.settingsCont.SetActive(false);
            GuiManager.instance.pauseButtonsCont.SetActive(true);
        }
        else
        {
            PausePanelMenuProvider.instance.mm_settingsCont.SetActive(false);
            PausePanelMenuProvider.instance.mm_buttonsCont.SetActive(true);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void ResetSettings()
    {
        PlayerPrefs.SetFloat("Volume", defaultVolume);
        PlayerPrefs.SetFloat("Sens", defaultSens);


        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            AudioListener.volume = defaultVolume;
            SettingsField.gameVolume.SetValue(0f, 1f, AudioListener.volume);

            Player.instance.controller.sens = defaultSens;
            SettingsField.gameSens.SetValue(0f, maxSens, Player.instance.controller.sens);
        }
        else
        {
            AudioListener.volume = defaultVolume;
            PausePanelMenuProvider.instance.mm_volume.SetValue(0f, 1f, AudioListener.volume);

            PausePanelMenuProvider.instance.mm_sens.SetValue(0f, maxSens, defaultSens);
        }
    }

    public void SetSens()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Player.instance.controller.sens = SettingsField.gameSens.slider.value * maxSens;
            SettingsField.gameSens.SetProcents();

            PlayerPrefs.SetFloat("Sens", Player.instance.controller.sens);
        }
        else
        {
            float _sens = PausePanelMenuProvider.instance.mm_sens.slider.value * maxSens;
            PausePanelMenuProvider.instance.mm_sens.SetProcents();

            PlayerPrefs.SetFloat("Sens", _sens);
        }
    }

    public void SetVolume()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            AudioListener.volume = SettingsField.gameVolume.slider.value;
            SettingsField.gameVolume.SetProcents();

            PlayerPrefs.SetFloat("Volume", AudioListener.volume);
        }
        else
        {
            AudioListener.volume = PausePanelMenuProvider.instance.mm_volume.slider.value;
            PausePanelMenuProvider.instance.mm_volume.SetProcents();

            PlayerPrefs.SetFloat("Volume", AudioListener.volume);
        }
    }
}
