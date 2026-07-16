using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public SettingsField sens;
    public SettingsField volume;

    private void Awake()
    {
        SettingsField.gameSens = sens;
        SettingsField.gameVolume = volume;
    }
}
