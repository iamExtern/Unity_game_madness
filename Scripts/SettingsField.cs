using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsField : MonoBehaviour
{
    public static SettingsField gameSens = null;
    public static SettingsField gameVolume = null;

    public TMP_Text propertyName;
    public TMP_Text procents;
    public Slider slider;

    public void SetValue(float min, float max, float current)
    {
        slider.value = MathExtra.FromLerp(min, max, current);
        SetProcents();
    }

    public void SetProcents()
    {
        procents.text = (int)(slider.value * 100f) + "%";
    }
}
