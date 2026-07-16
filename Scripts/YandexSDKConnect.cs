using UnityEngine;
using YG;

public class YandexSDKConnect : MonoBehaviour
{
    public static YandexSDKConnect instance = null;

    public static bool invokingGetData = false;
    private bool startAdShow = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (YandexGame.SDKEnabled)
                GetData();
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!startAdShow)
        {
            YandexGame.FullscreenShow();
            startAdShow = true;
        }
    }

    private void GetData()
    {
        if (!invokingGetData)
        {
            invokingGetData = true;
            switch (YandexGame.EnvironmentData.language)
            {
                case "ru":
                    MultiLanguage.lang = MultiLanguage.Language.Ru;
                    break;
                default:
                    MultiLanguage.lang = MultiLanguage.Language.En;
                    break;
            }

            Options.pc = YandexGame.EnvironmentData.deviceType == "desktop";

            if (PausePanel.instance != null)
                PausePanel.instance.MainMenuLocalization();
        }
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += GetData;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= GetData;
    }
}
