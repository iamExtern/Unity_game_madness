using TMPro;
using UnityEngine;

public class PausePanelMenuProvider : MonoBehaviour
{
    public static PausePanelMenuProvider instance;

    public GameObject mm_buttonsCont;
    public GameObject mm_settingsCont;

    public SettingsField mm_sens;
    public SettingsField mm_volume;

    public TMP_Text playText, settingsText, volumeText, sensText, backText, resetText, loadingText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PausePanel.instance.UnPause();
        Door.doors = new();
        Door.lockCodeDoors = new();
        ItemZone.itemZones = new();
        StateProvider.playerInExitZone = false;
        StateProvider.codeLockOpen = false;
    }
}
